package com.yachiyo.Config;

import lombok.RequiredArgsConstructor;
import org.springframework.ai.chat.client.ChatClient;
import org.springframework.ai.ollama.OllamaChatModel;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

@Configuration
@RequiredArgsConstructor
public class AIConfig {

    final OllamaChatModel model;

    @Bean
    public ChatClient chatClient() {
        return ChatClient.builder(model)
                .build();
    }
}
