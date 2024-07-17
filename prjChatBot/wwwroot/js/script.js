function toggleChat() {
    const chatBox = document.getElementById('chat-box');
    const iconImg = document.getElementById('icon-img');

    if (chatBox.style.display === 'none' || chatBox.style.display === '') {
        chatBox.style.display = 'flex';
        iconImg.src = '/images/close-icon.png'; // 替換成叉叉的圖案
    } else {
        chatBox.style.display = 'none';
        iconImg.src = '/images/chatbot-icon.png'; // 替換回智能客服機器人的圖案
    }
}

function toggleMenu() {
    const menuBox = document.getElementById('menu-box');
    if (menuBox.style.display === 'none' || menuBox.style.display === '') {
        menuBox.style.display = 'block';
    } else {
        menuBox.style.display = 'none';
    }
}

function sendMenuItemMessage(element) {
    const chatInput = document.getElementById('chat-input');
    chatInput.value = element.innerText;
    sendMessage();
}

function sendMessage() {
    const chatInput = document.getElementById('chat-input');
    const chatContent = document.getElementById('chat-content');

    if (chatInput.value.trim() !== '') {
        const userMessage = document.createElement('div');
        userMessage.classList.add('message', 'user');
        userMessage.innerText = chatInput.value;
        chatContent.appendChild(userMessage);

        // 清空輸入框
        const userMessageText = chatInput.value;
        chatInput.value = '';

        // 顯示點點點動畫
        const typingIndicator = document.createElement('div');
        typingIndicator.classList.add('message', 'bot', 'typing-indicator');
        typingIndicator.id = 'typing-indicator';
        typingIndicator.innerHTML = '<span></span><span></span><span></span>';
        chatContent.appendChild(typingIndicator);

        // 自動滾動到最新訊息
        chatContent.scrollTop = chatContent.scrollHeight;

        // 调用sendRequest函数来处理API请求
        sendRequest(userMessageText);
    }
}

async function sendRequest(message) {
    const url = 'http://localhost:3000/bot/8948ae93-ed70-44e9-9217-376c22b7a4ac/api';
    const apiKey = 'sk_db_I1kNRZPCU92bhzeGVvzyKbaesqzMM2nT';
    const data = {
        message: message,
        history: [],
        stream: true
    };

    try {
        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'x-api-key': apiKey
            },
            body: JSON.stringify(data)
        });

        if (!response.ok) {
            throw new Error(`HTTP error! Status: ${response.status}`);
        }

        const reader = response.body.getReader();
        const decoder = new TextDecoder('utf-8');
        let responseText = '';
        let historyContent = '';

        while (true) {
            const { done, value } = await reader.read();
            if (done) break;
            responseText += decoder.decode(value, { stream: true });
            const events = responseText.split('\n\n');
            events.forEach(eventString => {
                if (eventString.trim() !== '') {
                    const eventParts = eventString.split('\n');
                    const event = eventParts[0].replace('event: ', '');
                    const data = JSON.parse(eventParts[1].replace('data: ', ''));
                    if (event === 'result' && data.bot) {
                        // 移除「輸入中...」的符號
                        const typingIndicator = document.getElementById('typing-indicator');
                        if (typingIndicator) {
                            typingIndicator.remove();
                        }

                        const botMessage = document.createElement('div');
                        botMessage.classList.add('message', 'bot');
                        botMessage.innerText = data.bot.text;
                        document.getElementById('chat-content').appendChild(botMessage);

                        // 添加反馈功能
                        const feedbackContainer = document.createElement('div');
                        feedbackContainer.classList.add('feedback-container');

                        const feedbackText = document.createElement('span');
                        feedbackText.innerText = '這回答是否對你有幫助?';

                        const thumbsUp = document.createElement('span');
                        thumbsUp.innerText = '👍';
                        thumbsUp.classList.add('feedback-icon');
                        thumbsUp.addEventListener('click', () => {
                            alert('感謝您的反饋！');
                        });

                        const thumbsDown = document.createElement('span');
                        thumbsDown.innerText = '👎';
                        thumbsDown.classList.add('feedback-icon');
                        thumbsDown.addEventListener('click', () => {
                            alert('感謝您的反饋！');
                        });

                        feedbackContainer.appendChild(feedbackText);
                        feedbackContainer.appendChild(thumbsUp);
                        feedbackContainer.appendChild(thumbsDown);

                        document.getElementById('chat-content').appendChild(feedbackContainer);

                        // 自动滚动到最新消息
                        document.getElementById('chat-content').scrollTop = document.getElementById('chat-content').scrollHeight;
                        historyContent = data.history.map(item => `${item.type}: ${item.text}`).join('\n');
                        document.getElementById('history').innerText = historyContent;



                        // 自动滚动到最新消息
                        document.getElementById('chat-content').scrollTop = document.getElementById('chat-content').scrollHeight;
                    }
                }
            });
        }
    } catch (error) {
        console.error('Error:', error);
        document.getElementById('botResponse').innerText = error;
    }
}

function refreshChat() {
    const chatContent = document.getElementById('chat-content');
    chatContent.innerHTML = ''; // 清空對話內容

    // 添加最初的 Message bot 訊息
    const initialBotMessage = document.createElement('div');
    initialBotMessage.classList.add('message', 'bot');
    initialBotMessage.innerText = '您好，有什麼我可以幫助您的嗎？';
    chatContent.appendChild(initialBotMessage);
}

document.getElementById('chat-input').addEventListener('keydown', function (e) {
    if (e.key === 'Enter') {
        e.preventDefault();
        sendMessage();
    }
});
