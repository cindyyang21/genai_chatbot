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

//function sendMessage() {
//    const chatInput = document.getElementById('chat-input');
//    const chatContent = document.getElementById('chat-content');

//    if (chatInput.value.trim() !== '') {
//        const userMessage = document.createElement('div');
//        userMessage.classList.add('message', 'user');
//        userMessage.innerText = chatInput.value;
//        chatContent.appendChild(userMessage);

//        // 清空輸入框
//        chatInput.value = '';

//        // 自動滾動到最新訊息
//        chatContent.scrollTop = chatContent.scrollHeight;

//        // 模拟机器人回应
//        setTimeout(() => {
//            const botMessage = document.createElement('div');
//            botMessage.classList.add('message', 'bot');

//            const botText = document.createElement('span');
//            botText.innerText = '我不知道你在說什麼';

//            botMessage.appendChild(botText);

//            chatContent.appendChild(botMessage);

//            // 自动滚动到最新消息
//            chatContent.scrollTop = chatContent.scrollHeight;
//        }, 1000);
//    }
//}

//document.getElementById('chat-input').addEventListener('keydown', function (e) {
//    if (e.key === 'Enter') {
//        e.preventDefault();
//        sendMessage();
//    }
//});
