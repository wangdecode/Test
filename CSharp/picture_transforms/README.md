## 说明
这是一个用C#编写的简易图片转换程序，调用系统自带图片解析库。  

## 编译方法
Debug:   csc.exe -debug PicTransform.cs ProgressBar.cs  
Release: csc.exe PicTransform.cs ProgressBar.cs  

## 用法
把一个或多个文件夹拖到程序上  
可以手动添加一个同名的配置文件(.txt)，在配置文件中修改配置  

usage: [exe] paths  

__ReSizeWidth__ : 0 表示不调整图片大小，当值大于0时，调整图片宽度为此值并比例缩放  
__ImageFormat__ : 支持转换后的图片格式  
__JpgQuality__  : 当 ImageFormat 为 jpg 时，用于设置转换 jpg 的质量（范围0－100），数值越大文件体积越大，质量相对也越高  
__IsNewFile__   : true 表示创建一个新的文件夹存放原图。false 表示直接覆盖原文件。  

ReSizeWidth : only resize image while Width > 0  
ImageFormat : jpg、png、bmp、tiff、gif  
JpgQuality  : 0-100  
IsNewFile   : if true will make a new folder  

### Sample config file ( [exe].txt ):  
ReSizeWidth = 0  
ImageFormat = jpg  
JpgQuality  = 90  
IsNewFile   = false  
