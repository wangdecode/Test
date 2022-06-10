这是一个用C#编写的简易图片转换程序，调用系统自带图片解析库。<br>
<br>
编译方法<br>
Debug:   csc.exe -debug PicTransform.cs ProgressBar.cs<br>
Release: csc.exe PicTransform.cs ProgressBar.cs<br>
<br>
用法<br>
把一个或多个文件夹拖到程序上<br>
可以手动添加一个同名的配置文件(.ini)，在配置文件中修改配置<br>
<br>
usage: [exe] paths <br><br>

ReSizeWidth : only resize image while Width > 0<br>
ImageFormat : jpg、png、bmp、tiff、gif<br>
JpgQuality  : 0-100<br>
IsNewFile   : if true will make a new folder<br><br>

Sample ini file:<br>
ReSizeWidth = 0<br>
ImageFormat = jpg<br>
JpgQuality  = 90<br>
IsNewFile   = false<br>
