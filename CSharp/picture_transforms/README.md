这是一个用C#编写的简易图片转换程序，调用系统自带图片解析库。<br>
<br>
编译方法<br>
Debug:   csc.exe -debug [file]<br>
Release: csc.exe [file]<br>
<br>
用法<br>
把文件夹拖到程序上<br>
<br>
usage: [exe] path [ReSizeWidth] [ImageFormat] [quality]<br>
<br>
ReSizeWidth (default=0):<br>
only resize image while Width > 0<br>
<br>
ImageFormat (default=jpg):<br>
jpg、png、bmp、tiff、gif<br>
<br>
quality (default=90, jpg only):<br>
0-100<br>
<br>
