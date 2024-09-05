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
    toggleMenu();
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
    const url = 'http://localhost:3000/bot/bd041136-3064-4b6f-a160-1481f399d8be/api';
    const apiKey = 'sk_db_reCVt4ul5uNF70MVBjFUrrMfue4E4PsZ';
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
                        botMessage.innerHTML = marked.parse(data.bot.text); // 使用 marked.js 解析 Markdown

                        //所有超連結另開分頁
                        const links = botMessage.querySelectorAll('a');
                        links.forEach(link => {
                            link.setAttribute('target', '_blank');
                        });

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
                            openModal('up');
                        });

                        const thumbsDown = document.createElement('span');
                        thumbsDown.innerText = '👎';
                        thumbsDown.classList.add('feedback-icon');
                        thumbsDown.addEventListener('click', () => {
                            openModal('down');
                        });

                        feedbackContainer.appendChild(feedbackText);
                        feedbackContainer.appendChild(thumbsUp);
                        feedbackContainer.appendChild(thumbsDown);

                        document.getElementById('chat-content').appendChild(feedbackContainer);

                        // 創建模態彈窗
                        const modal = document.createElement('div');
                        modal.classList.add('modal');
                        modal.style.display = 'none'; // 初始隱藏

                        const modalContent = document.createElement('div');
                        modalContent.classList.add('modal-content');

                        const modalHeader = document.createElement('div');
                        modalHeader.classList.add('modal-header');
                        const closeButton = document.createElement('span');
                        closeButton.classList.add('close');
                        closeButton.innerHTML = '&times;';
                        closeButton.onclick = () => closeModal();
                        const modalTitle = document.createElement('h2');

                        modalHeader.appendChild(closeButton);
                        modalHeader.appendChild(modalTitle);

                        const modalBody = document.createElement('div');
                        modalBody.classList.add('modal-body');

                        // 添加提示文本
                        const feedbackPrompt = document.createElement('p');
                        modalBody.appendChild(feedbackPrompt);

                        const checkboxContainer = document.createElement('div');
                        checkboxContainer.classList.add('checkbox-container');

                        modalBody.appendChild(checkboxContainer);

                        const modalFooter = document.createElement('div');
                        modalFooter.classList.add('modal-footer');
                        const submitButton = document.createElement('button');
                        submitButton.classList.add('submit-btn');
                        submitButton.innerText = '提交';
                        submitButton.onclick = () => {
                            const checkedOptions = [];
                            checkboxContainer.querySelectorAll('input:checked').forEach(checkbox => {
                                checkedOptions.push(checkbox.value);
                            });
                            if (checkedOptions.length > 0) {
                                alert('感謝您的反饋!');
                            } else {
                                alert('請選擇至少一個選項。');
                            }
                            closeModal();
                        };

                        modalFooter.appendChild(submitButton);

                        modalContent.appendChild(modalHeader);
                        modalContent.appendChild(modalBody);
                        modalContent.appendChild(modalFooter);

                        modal.appendChild(modalContent);
                        document.body.appendChild(modal);

                        // 開啟彈窗
                        function openModal(type) {
                            let reasons = [];
                            if (type === 'up') {
                                modalTitle.innerText = '感謝您的支持！';
                                feedbackPrompt.innerText = '請給予我們反饋：';
                                feedbackPrompt.style.marginBottom = '5px';
                                reasons = [
                                    '智能客服回覆有解決問題',
                                    '智能客服使用穩定不會中斷',
                                    '智能客服操作介面簡單直覺好上手',
                                    '智能客服解決問題速度快',
                                    '智能客服回覆資料正確',
                                    '其他'
                                ];
                            } else if (type === 'down') {
                                modalTitle.innerText = '很抱歉未能幫助到您!';
                                feedbackPrompt.innerText = '請告訴我們原因：';
                                feedbackPrompt.style.marginBottom = '5px';
                                reasons = [
                                    '智能客服回覆沒有解決問題',
                                    '智能客服使用不穩定會中斷',
                                    '智能客服操作介面困難不易上手',
                                    '智能客服解決問題速度慢',
                                    '智能客服回覆資料錯誤',
                                    '其他'
                                ];
                            }



                            // 清除以前的選項
                            checkboxContainer.innerHTML = '';

                            // 創建兩個列容器
                            const column1 = document.createElement('div');
                            const column2 = document.createElement('div');
                            column1.style.width = '50%';
                            column1.style.float = 'left';
                            column2.style.width = '50%';
                            column2.style.float = 'left';

                            // 添加新的選項到兩個列容器中
                            reasons.forEach((reason, index) => {
                                const label = document.createElement('label');
                                const checkbox = document.createElement('input');
                                checkbox.type = 'checkbox';
                                checkbox.value = reason;

                                label.appendChild(checkbox);
                                label.appendChild(document.createTextNode(reason));

                                if (reason === '其他') {
                                    const otherText = document.createElement('input');
                                    otherText.type = 'text';
                                    otherText.placeholder = '請輸入其他原因';
                                    otherText.style.visibility = 'hidden'; // 初始隱藏
                                    label.appendChild(otherText);

                                    // 設置響應式寬度和邊框盒模型
                                    otherText.style.width = '100%';
                                    otherText.style.boxSizing = 'border-box';

                                    // 設置只顯示下邊框
                                    otherText.style.border = 'none';           // 先隱藏所有邊框
                                    otherText.style.borderBottom = '1px solid #000'; // 顯示下邊框，可以根據需要更改顏色和寬度


                                    const breakLine = document.createElement('br'); // 新增換行符
                                    label.appendChild(breakLine);
                                    label.appendChild(otherText);

                                    // 監聽「其他」選項變化
                                    checkbox.addEventListener('change', () => {
                                        if (checkbox.checked) {
                                            otherText.style.visibility = 'visible';
                                            otherText.value = '';
                                        } else {
                                            otherText.style.visibility = 'hidden';
                                        }
                                    });
                                }

                                if (index < Math.ceil(reasons.length / 2)) {
                                    column1.appendChild(label);
                                } else {
                                    column2.appendChild(label);
                                }
                            });

                            // 將兩列容器添加到checkboxContainer中
                            checkboxContainer.appendChild(column1);
                            checkboxContainer.appendChild(column2);

                            modal.style.display = 'block';
                        }

                        // 關閉彈窗
                        function closeModal() {
                            modal.style.display = 'none';
                        }


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

    // 添加精選商品的 Message bot 訊息
    const productMessage = document.createElement('div');
    productMessage.classList.add('message', 'bot');
    productMessage.innerText = '以下為精選商品，提供您參考!';
    chatContent.appendChild(productMessage);
}

document.getElementById('chat-input').addEventListener('keydown', function (e) {
    if (e.key === 'Enter') {
        e.preventDefault();
        sendMessage();
    }
});
