import sys, os, binascii


def fwrite(data, name, code="UTF-8", error="strict", mode="wb"):
    file = open(name, mode)
    file.write(data)
    file.close()

if __name__ == "__main__":
    
    # filein_name = r'test.pfs'
    filein_name = input('请输入文件：')
    
    filein = open(filein_name.replace('\"',''), 'rb')
    # 读取文件头
    filehead = filein.read(4)
    # 判断是否为 'pf' 开头
    if filehead[0:2] == b'\x70\x66':
        print('文件头：' + filehead[0:3].decode('latin-1'))
        ver = int(binascii.b2a_hex( filehead[2:3] ), 16)
        # 第三位不是 0-9
        if ver < 48 or ver > 57:
            exit()
        pos = 0
    # 判断是否为可执行文件，若是读取前 50 MB
    elif filehead[0:2] == b'\x4D\x5A':
        pos = filein.read(1024 * 1024 * 50).find(b'\x70\x66\x36')
    # 跳过文件头
    filein.seek(pos + 3)
    # 跳过 4 Byte
    filein.seek(4, 1)
    # 读取文件数量
    file_num = int(binascii.b2a_hex( filein.read(4)[::-1] ), 16)
    
    # 保存文件索引，[[文件名, 偏移, 大小], ...]
    filelist = []
    
    print('文件数量：' + str(file_num))
    for i in range(file_num):
        # 文件名长度
        fileout_name_len = int(binascii.b2a_hex( filein.read(4)[::-1] ), 16)
        # 文件名
        fileout_name = filein.read(fileout_name_len).decode('utf-8')
        # 文件偏移开始
        fileout_pos = int(binascii.b2a_hex( filein.read(4)[::-1] + filein.read(4)[::-1] ), 16)
        # 文件大小
        fileout_len = int(binascii.b2a_hex( filein.read(4)[::-1] ), 16)
        # 添加到文件索引
        filelist.append([fileout_name, fileout_pos, fileout_len])
        print(fileout_name)
    
    input('输入任意字符输出')
    
    for f in filelist:
        print(f[0])
        pos = f[0].rfind('\\')
        if pos != -1:
            path = f[0][:pos]
            if not os.path.exists(path):
                os.makedirs(path)
                
        filein.seek(f[1])
        data = filein.read(f[2])
        
        fwrite(data, f[0])
    
    input('完成')
