这是一个用C#编写的简易图片转换程序，调用系统自带图片解析库。<br>
<br>
编译方法<br>
Debug:   csc.exe -debug [file]<br>
Release: csc.exe [file]<br>
<br>
用法<br>
把一个或多个文件夹拖到程序上<br>
可以手动添加一个同名的配置文件(.ini)，在配置文件中修改配置<br>
<br>
usage: [exe] paths <br>
<br>
ReSizeWidth (default=0): only resize image while Width > 0<br><br>
ImageFormat (default=jpg): jpg、png、bmp、tiff、gif<br><br>
JpgQuality (default=90, jpg only): 0-100<br>
<br><br>
Sample ini:<br>
ReSizeWidth = 0<br>
ImageFormat = jpg<br>
JpgQuality  = 90<br>
