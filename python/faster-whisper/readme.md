# 说明   
基于 faster-whisper 实现的简单API接口调用样例。
    
`faster-whisper-local-demo` 一个调用本地 faster-whisper 模型的测试样例   
    
`web_api` 一个将本地 faster-whisper 模型封装成 web api 接口的样例   
`whisper_api` 本地 faster-whisper 类，提供语音转文本功能   

## 最简单的 whisper_api 调用方法
```python
from whisper_api import whisper

mywhisper = whisper()
result = mywhisper.audio2text('1.mp3')
```
