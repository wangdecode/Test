from fastapi import FastAPI, File, UploadFile, Request
import uvicorn as uvicorn # api web 服务
from whisper_api import whisper # 导入 whisper 库

app = FastAPI()
mywhisper =  None

@app.get("/")
def read_root():
    return {"ver": "0.5"}


# @app.get("/id/{input_id}")
# def read_item(input_id: int, input_str: str = None):
    # return {"id": input_id, "str": input_str}


@app.post('/audio2text')
async def get_audio_file(
    file: UploadFile = File(...)
):
    """
        上传语音文件并转录为文本
        - **file**： 要上传的文件
        
        返回结果
        - **file**： 上传后的文件及路径
        - **text**： 语音转换后的文本
        - **time**： 语音转换时长
    """
    contents = await file.read()
    # 文件名去除特殊字符
    filepath = "./tmp/" + file.filename.translate(str.maketrans('', '', '@#$%^&*()_+/\\')).replace('..','')
    with open(filepath, "wb") as f:
        f.write(contents)
    
    result = ('','')
    if (mywhisper != None):
        result = mywhisper.audio2text(filepath)
    
    return ({'file' : filepath,
             'text' : result[0],
             'time' : result[1]})


@app.post('/audio2text2')
async def get_audio_file2(request: Request):
    file_data = await request.body()
    
    filepath = "./tmp/tmp"
    with open(filepath, "wb") as f:
        f.write(file_data)
    
    result = ('','')
    if (mywhisper != None):
        result = mywhisper.audio2text(filepath)
    
    return ({'file' : filepath,
             'text' : result[0],
             'time' : result[1]})


if __name__ == "__main__":
    mywhisper = whisper()
    uvicorn.run(app=app, host="127.0.0.1", port=8000)
    
