## 测试用例，配合多线程脚本使用
import os, sys, time, random

if __name__ == "__main__":
    output = ''
    
    args = sys.argv[1:]
    random_time = random.randint(0, 30) / 10
    
    if(len(args) > 0):
        output = args[0]
    
    time.sleep(random_time)
    print(output + ' -> ' + str(random_time) + 's')
