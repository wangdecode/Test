## python 多线程样例
import os, subprocess, concurrent.futures

# 最大并发数
max_count = 3


# 并发执行任务
def task(n, args):
    print(f"Task {n} is running")
    
    # program = 'ffmpeg'
    # args = [' -i 01.ogg out\01.ogg']
    program = 'python'
    args = ['test.py',args]
    
    result = subprocess.run([program] + args, capture_output=True, text=True)
    
    if(result.returncode == 1):
        print(result.stderr)
    
    return result.stdout.strip()


# 使用线程池并发执行任务
def use_thread_pool():
    
    with concurrent.futures.ThreadPoolExecutor(max_workers=max_count) as executor:
        future_to_task = {}
        sample_task_num = 5
        
        filename = ['file ' + str(i) for i in range(sample_task_num)]
        
        # 提交单个任务
        # future_to_task[executor.submit(task, 1)] = 1
        # 提交任务到线程池中
        for i in range(sample_task_num):
            future_to_task[executor.submit(task, i, filename[i])] = i
        
        # 获取并打印结果
        for future in concurrent.futures.as_completed(future_to_task):
            task_number = future_to_task[future]
            try:
                data = future.result()
                print(f"Task {task_number} result: {data}")
            except Exception as e:
                print(f"Task {task_number} error: {e}")


# 调用函数，开始执行
use_thread_pool()
