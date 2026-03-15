package com.yachiyo.Config;

import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.mock.web.MockMultipartFile;
import org.springframework.web.multipart.MultipartFile;

import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Paths;

@Configuration
public class IOFileConfig {

    /**
     * 运行时路径
     */
    public static final String RUNTIME_FILE_PATH = System.getProperty("user.dir")+"/Common";

    /**
     * 上传文件路径
     */
    public static final String UPLOAD_FILE_PATH = RUNTIME_FILE_PATH + "/src/main/resources/static/upload/";

    /**
     * 保存文件路径
     */
    public static final String SAVE_FILE_PATH = RUNTIME_FILE_PATH + "/src/main/resources/static/save/";

    /**
     * 保存文件
     */
    public boolean saveFile(String fileName, MultipartFile fileBytes) {
        try {
            fileBytes.transferTo(Paths.get(SAVE_FILE_PATH + fileName));
            return true;
        } catch (IOException e) {
            e.printStackTrace();
            return false;
        }
    }

    /**
     * 上传文件
     */
    public boolean uploadFile(String fileName, MultipartFile fileBytes) {
        try {
            fileBytes.transferTo(Paths.get(UPLOAD_FILE_PATH + fileName));
            return true;
        } catch (IOException e) {
            e.printStackTrace();
            return false;
        }
    }

    /**
     * 读取文件
     */
    public byte[] readFile(String fileName) {
        try {
            return Files.readAllBytes(Paths.get(UPLOAD_FILE_PATH + fileName));
        } catch (IOException e) {
            return null;
        }
    }

    /**
     * 删除文件
     */
    public boolean deleteFile(String fileName) {
        try {
            Files.delete(Paths.get(UPLOAD_FILE_PATH + fileName));
            return true;
        } catch (IOException e) {
            e.printStackTrace();
            return false;
        }
    }

    /**
     * 检查文件是否存在
     */
    public boolean checkFileExist(String fileName) {
        return Files.exists(Paths.get(UPLOAD_FILE_PATH + fileName));
    }


    /**
     * 检查目录是否存在
     */
    public boolean checkDirExist(String dirName) {
        return Files.exists(Paths.get(UPLOAD_FILE_PATH + dirName));
    }

    /**
     * 创建目录
     */
    public void createDir(String s) {
        try {
            Files.createDirectory(Paths.get(UPLOAD_FILE_PATH + s));
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    @Bean
    public IOFileConfig ioFileConfig() {
        return new IOFileConfig();
    }
}