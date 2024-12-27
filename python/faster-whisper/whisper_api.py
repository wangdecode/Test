import time
from faster_whisper import WhisperModel

class whisper:

    model_path = "large-v2"
    default_lang = "zh" # zh,ja,en
    model = None

    # vad 默认参数
    vad=dict(threshold=0.5,   # 歌词可以改小点，如0.1
        min_speech_duration_ms=250,
        max_speech_duration_s=float("inf"),
        min_silence_duration_ms=2000,
        speech_pad_ms=400)
    
    def __init__(self):
        print('Init:     whisper load..')
        self.model = WhisperModel(model_size_or_path=self.model_path, device="cuda", compute_type="float16", local_files_only=True)
        print('Init:     finish.')
    
    def audio2text(self, input_file, lang = default_lang):
    
        if self.model == None:
            return

        start_time = time.time() # 计时开始

        # 语音转文字
        segments, info = self.model.transcribe(input_file, beam_size=5, language=lang,
            vad_filter=True, vad_parameters=self.vad)
        print("Detected language '%s' with probability %f" % (info.language, info.language_probability))

        # 过滤重复内容
        tmp = ""
        result = []
        output_file = input_file + '.txt'
        for segment in segments:
            if(tmp == segment.text):
                continue
            tmp = segment.text
            # result.append( "[%.2fs -> %.2fs] %s \n" % (segment.start, segment.end, segment.text) )
            result.append( "%s \n" % (segment.text) )
        
        result = ''.join(result)
        
        end_time = time.time() # 计时结束
        
        return (result, "%.2f" % (end_time - start_time))
