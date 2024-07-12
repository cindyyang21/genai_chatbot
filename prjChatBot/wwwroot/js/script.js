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
        chatInput.value = '';

        // 自動滾動到最新訊息
        chatContent.scrollTop = chatContent.scrollHeight;

        // 模拟机器人回应
        setTimeout(() => {
            const botMessage = document.createElement('div');
            botMessage.classList.add('message', 'bot');

            const botText = document.createElement('span');
            botText.innerText = '我不知道你在說什麼';

            botMessage.appendChild(botText);

            chatContent.appendChild(botMessage);

            // 添加小字及讚和倒讚
            const feedbackContainer = document.createElement('div');
            feedbackContainer.classList.add('feedback-container');

            const feedbackText = document.createElement('span');
            feedbackText.innerText = '這回答是否對你有幫助?';

            const thumbsUp = document.createElement('span');
            thumbsUp.innerText = '👍'; // 讚的表情符號
            thumbsUp.classList.add('feedback-icon');
            thumbsUp.addEventListener('click', () => {
                alert('感謝您的反饋！');
                // 在此处可以添加更多处理逻辑，例如记录反馈
            });

            const thumbsDown = document.createElement('span');
            thumbsDown.innerText = '👎'; // 倒讚的表情符號
            thumbsDown.classList.add('feedback-icon');
            thumbsDown.addEventListener('click', () => {
                alert('感謝您的反饋！');
                // 在此处可以添加更多处理逻辑，例如记录反馈
            });

            feedbackContainer.appendChild(feedbackText);
            feedbackContainer.appendChild(thumbsUp);
            feedbackContainer.appendChild(thumbsDown);

            chatContent.appendChild(feedbackContainer);

            // 自动滚动到最新消息
            chatContent.scrollTop = chatContent.scrollHeight;
        }, 1000);
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