# ChatBot前端

## 安裝

Clone the repository:

```bash
git clone https://github.com/cindyyang21/genai_chatbot.git
```

到專案資料夾內運行 docker-compose file:

```bash
docker-compose up --build
```

如果有修改原始碼需要重新建置，依序執行下列指令
```bash
docker-compose down -v
```
```bash
docker-compose up --build
```
## 內容設定

### 必要設定
![image](https://github.com/cindyyang21/genai_chatbot/blob/main/ReadMeImage/%E5%BF%85%E8%A6%81%E8%A8%AD%E5%AE%9A.png?raw=true)

1.設定名字與對話框顏色
![image](https://github.com/cindyyang21/genai_chatbot/blob/main/ReadMeImage/%E5%90%8D%E7%A8%B1%E8%88%87%E9%A1%8F%E8%89%B2.png?raw=true)
2.連接機器人
![image](https://github.com/cindyyang21/genai_chatbot/blob/main/ReadMeImage/api%E8%88%87iframe.png?raw=true)
3.設定圖示(需先設定機器人圖示才會顯示內容)
![image](https://github.com/cindyyang21/genai_chatbot/blob/main/ReadMeImage/icon%E5%9C%96%E7%A4%BA.png?raw=true)
# 
### 其他功能
![image](https://github.com/cindyyang21/genai_chatbot/blob/main/ReadMeImage/%E5%85%B6%E4%BB%96%E5%8A%9F%E8%83%BD.png?raw=true)

1.新增初始訊息

![image](https://github.com/cindyyang21/genai_chatbot/blob/main/ReadMeImage/%E5%88%9D%E5%A7%8B%E8%A8%8A%E6%81%AF.png?raw=true)

2.新增卡片(卡片可以設定圖片及超連結文字)

![image](https://github.com/cindyyang21/genai_chatbot/blob/main/ReadMeImage/%E5%8D%A1%E7%89%87%E7%AF%84%E4%BE%8B.png?raw=true)

3.新增選單(選單為常見功能快捷發話，將預設文字發送給機器人)

![image](https://github.com/cindyyang21/genai_chatbot/blob/main/ReadMeImage/%E9%81%B8%E5%96%AE%E7%AF%84%E4%BE%8B.png?raw=true)

4.用戶回饋數據

![image](https://github.com/cindyyang21/genai_chatbot/blob/main/ReadMeImage/%E5%9B%9E%E9%A5%8B.png?raw=true)
![image](https://github.com/cindyyang21/genai_chatbot/blob/main/ReadMeImage/%E5%9B%9E%E9%A5%8B1.png?raw=true)

## 開發

### 預覽

將網址Admin及後面改為Home/Bot即可預覽

### 資料庫

連線地址為localhost,1433
帳號密碼設定於Program.cs
