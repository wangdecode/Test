#!/usr/bin/python
# -*- coding: utf-8 -*-

import pyapi
import sys, os, re, time, random


def dl_urls(url):
    index = 1
    
    data = pyapi.gethttp(url,'gbk')
    # test
    # pyapi.fwrite(data, '1.txt')
    # data = pyapi.fread('1.txt')

    author = re.search(r'(?<=author\" content=\").+?(?=\")', data, re.DOTALL).group()
    book_name = re.search(r'(?<=book_name\" content=\").+?(?=\")', data, re.DOTALL).group()
    
    data = re.search(r'(?<=\<div class=\"article_texttitleb\"\>).+?(?=\</div\>)', data, re.DOTALL).group()
    urls = re.compile(r'\<a href=\"(.+?)\"\>(.+?)\</', re.DOTALL).findall(data)
    
    folder_path = make_folder(book_name, author)
    
    if urls == []:
        return
    
    url_len = '/' + str(len(urls)) + ' '
    
    for u in urls:
        name = get_name(u[1], index)
        url_new = re.search(r'\w+://.+?(?=/)', url, re.DOTALL).group() + u[0]
        
        data = pyapi.gethttp(url_new, 'gbk')
        dl_text(data, folder_path + name)
        
        print(str(index) + url_len + u[1])
        
        # sleep random time
        time.sleep(round(random.uniform(0.6, 3.0),3))
        index = index + 1
    
    return


def make_folder(book_name, author):
    folder_name = book_name + '-' + author
    
    if not os.path.exists(folder_name):
        os.makedirs(folder_name)
    
    return (folder_name + '\\')


def get_name(name, index):
    name = str(index).rjust(5,'0') + '-' + name + '.txt'
    return name


def dl_text(data, name):
    data = re.search(r'(?<=\<div id=\"chapterContent\"\>).+?(?=(\</div\>|\<script\>))', data, re.DOTALL).group()
    data = data.replace('&nbsp;','').replace('&amp;','').replace('quot;','').replace(' ','')
    data = data.replace('<br/>\r\n<br/>\r\n','\r\n    ').strip()
    data = data + '\r\n\r\n'
    # print(data)
    pyapi.fwrite(data, name)
    return


if __name__ == "__main__":
    url = ''
    dl_urls(url)
    print('finish.')
    
