package com.yachiyo.service;

import com.yachiyo.dto.ConversationResponse;
import com.yachiyo.dto.PromptResponse;
import com.yachiyo.result.Result;

import java.util.List;

public interface HistoryService {

    /**
     * 获取会话记忆
     * @param conservationId 会话ID
     * @return Result<List<PromptRequest>>
     */
    Result<List<PromptResponse>> getHistory(String conservationId);

    /**
     * 获取会话列表
     *
     * @return Result<List<Integer>>
     */
    Result<List<ConversationResponse>> getConservationIds();


}
