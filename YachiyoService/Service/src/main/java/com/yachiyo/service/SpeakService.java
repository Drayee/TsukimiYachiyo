package com.yachiyo.service;


public interface SpeakService {

     /**
     * 文本合成语音方法
     * @param text 待合成语音的文本
     * @return 合成后的语音文件路径
     */
    byte[] textToSpeech(String text);

}
