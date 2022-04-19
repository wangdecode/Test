这是一个用C#编写的简易图片转换程序，调用系统自带图片解析库。<br>

编译方法
Debug:   csc.exe -debug [file]
Release: csc.exe [file]

用法
把文件夹拖到程序上

usage: [exe] path [ReSizeWidth] [ImageFormat] [quality]

ReSizeWidth (default=0):
only resize image while Width > 0

ImageFormat (default=jpg):
jpg、png、bmp、tiff、gif

quality (default=90, jpg only):
0-100
