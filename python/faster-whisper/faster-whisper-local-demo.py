import time
from faster_whisper import WhisperModel

input_file = "1.mp3"
output_file = "out.txt"

model_path = "large-v2" # 指定目录，目录下放模型所有文件
lang = "ja" # zh,ja,en

# vad 默认参数
vad=dict(threshold=0.5,
    min_speech_duration_ms=250,
    max_speech_duration_s=float("inf"),
    min_silence_duration_ms=2000,
    speech_pad_ms=400)
    
start_time = time.time() # 计时开始

# Run on GPU
model = WhisperModel(model_size_or_path=model_path, device="cuda", compute_type="float16", local_files_only=True)
# GPU with INT8
# model = WhisperModel(model_size_or_path=model_path, device="cuda", compute_type="int8_float16", local_files_only=True)
# CPU with INT8
# model = WhisperModel(model_size_or_path=model_path, device="cpu", compute_type="int8", local_files_only=True)

if lang == "":
    segments, info = model.transcribe(input_file, beam_size=5, vad_filter=True, vad_parameters=vad) # 自动判断语言
    print("Detected language '%s' with probability %f" % (info.language, info.language_probability))
else:
    segments, info = model.transcribe(input_file, beam_size=5, language=lang,
        vad_filter=True, vad_parameters=vad)# 指定语言

# for segment in segments:
    # print("[%.2fs -> %.2fs] %s" % (segment.start, segment.end, segment.text))
    # # print(" " * 19 + "|")

# 过滤重复内容（待完善）
tmp = ""
with open(output_file, 'w', encoding='utf-8') as file:
    for segment in segments:
        if(tmp == segment.text):
            continue
        tmp = segment.text
        info = "[%.2fs -> %.2fs] %s" % (segment.start, segment.end, segment.text)
        print(info)
        # print(" " * 19 + "|")
        try:
            file.write(info + "\n")
        except UnicodeEncodeError:
            print('-----------------------------------------------')
            continue

end_time = time.time() # 计时结束
print("运行时间: %.2f 秒" % (end_time - start_time))
