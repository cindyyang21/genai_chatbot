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
    const apiKey = 'sk_db_SbHAI27Ve42urLpEIL6WyBs21vQd2GCs';
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
                        // 將 messageId 設為機器人的回覆文字，並且處理特殊字符
                        const botText = data.bot.text.trim().replace(/\s+/g, ' ');  // 移除多餘的空格或換行符
                        botMessage.setAttribute('data-id', botText);
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

                        // 將訊息ID作為參數傳遞給openModal
                        thumbsUp.addEventListener('click', () => {
                            const messageId = botMessage.getAttribute('data-id'); // 假設每條訊息有data-id
                            openModal('up', messageId); // 傳入訊息ID
                        });

                        const thumbsDown = document.createElement('span');
                        thumbsDown.innerText = '👎';
                        thumbsDown.classList.add('feedback-icon');

                        // 同樣，將訊息ID作為參數傳遞
                        thumbsDown.addEventListener('click', () => {
                            const messageId = botMessage.getAttribute('data-id');
                            openModal('down', messageId);
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
                            let otherReasonText = '';

                            checkboxContainer.querySelectorAll('input:checked').forEach(checkbox => {
                                if (checkbox.value === '其他') {
                                    const otherText = checkbox.closest('label').querySelector('input[type="text"]')?.value || '';
                                    if (otherText.trim() !== '') {
                                        otherReasonText = otherText;
                                    }
                                } else {
                                    checkedOptions.push(checkbox.value);
                                }
                            });

                            // 從彈窗中取得messageId
                            const messageId = modal.getAttribute('data-message-id');

                            if (checkedOptions.length > 0 || otherReasonText) {
                                // 發送資料到後端
                                fetch('/Admin/SubmitFeedback', {
                                    method: 'POST',
                                    headers: {
                                        'Content-Type': 'application/json'
                                    },
                                    body: JSON.stringify({
                                        reasons: checkedOptions,
                                        otherReason: otherReasonText,
                                        feedbackType: modalTitle.innerText === '感謝您的支持！' ? 'up' : 'down',
                                        chatbotMessage: messageId  // 傳送具體的訊息 ID，這裡是回覆的文字
                                    })
                                }).then(response => {
                                    if (response.ok) {
                                        alert('感謝您的反饋!');
                                        closeModal();
                                    } else {
                                        alert('提交失敗，請稍後再試。');
                                    }
                                }).catch(error => {
                                    alert('提交失敗，請稍後再試。');
                                });
                            } else {
                                alert('請選擇至少一個選項。');
                            }
                        };



                        modalFooter.appendChild(submitButton);

                        modalContent.appendChild(modalHeader);
                        modalContent.appendChild(modalBody);
                        modalContent.appendChild(modalFooter);

                        modal.appendChild(modalContent);
                        document.body.appendChild(modal);

                        // 開啟彈窗
                        function openModal(type, messageId) {
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
                            // 傳入messageId並保存在隱藏元素或全域變數中供提交使用
                            modal.setAttribute('data-message-id', messageId);
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

    // 發送請求獲取初始訊息
    const initialMessagesPromise = fetch('/Admin/GetInitialMessages').then(response => response.json());

    // 發送請求獲取產品卡片資料
    const productCardsPromise = fetch('/Admin/GetProductCards').then(response => response.json());

    // 使用 Promise.all 來等待兩個請求都完成
    Promise.all([initialMessagesPromise, productCardsPromise])
        .then(([initialMessages, productCards]) => {
            // 1. 處理初始訊息
            if (initialMessages.length > 0) {
                initialMessages.forEach(message => {
                    const messageDiv = document.createElement('div');
                    messageDiv.classList.add('message', 'bot');
                    messageDiv.innerText = message.message; // 將訊息顯示出來
                    chatContent.appendChild(messageDiv);
                });
            }

            // 2. 處理卡片資料
            if (productCards.length > 0) {
                // 建立輪播容器
                const carouselContainer = document.createElement('div');
                carouselContainer.id = "cardCarousel";
                carouselContainer.classList.add("carousel", "slide");

                // 當產品卡片數量大於1時才啟用輪播
                if (productCards.length > 1) {
                    carouselContainer.setAttribute("data-bs-ride", "carousel");
                }

                const carouselInner = document.createElement('div');
                carouselInner.classList.add("carousel-inner");

                // 生成每個卡片的 HTML 結構
                productCards.forEach((card, index) => {
                    const carouselItem = document.createElement('div');
                    carouselItem.classList.add("carousel-item");
                    if (index === 0) {
                        carouselItem.classList.add("active"); // 第一個卡片要設為 active
                    }

                    // 卡片內容
                    const cardDiv = document.createElement('div');
                    cardDiv.classList.add('card');
                    cardDiv.style.maxWidth = '100%';
                    cardDiv.style.margin = 'auto';

                    // 卡片圖片
                    const cardImg = document.createElement('img');
                    cardImg.src = `/images/${card.imageFileName}`;
                    cardImg.classList.add('card-img-top');
                    cardImg.alt = card.title;
                    cardDiv.appendChild(cardImg);

                    // 卡片 body
                    const cardBody = document.createElement('div');
                    cardBody.classList.add('card-body');
                    const cardTitle = document.createElement('h5');
                    cardTitle.classList.add('card-title');
                    cardTitle.innerText = card.title;
                    cardBody.appendChild(cardTitle);
                    cardDiv.appendChild(cardBody);

                    // 卡片鏈接
                    const listGroup = document.createElement('ul');
                    listGroup.classList.add('list-group', 'list-group-flush');

                    const listItem1 = document.createElement('li');
                    listItem1.classList.add('list-group-item');
                    const link1 = document.createElement('a');
                    link1.href = card.url1;
                    link1.target = '_blank';
                    link1.classList.add('card-link');
                    link1.innerText = card.name1;
                    listItem1.appendChild(link1);
                    listGroup.appendChild(listItem1);

                    const listItem2 = document.createElement('li');
                    listItem2.classList.add('list-group-item');
                    const link2 = document.createElement('a');
                    link2.href = card.url2;
                    link2.target = '_blank';
                    link2.classList.add('card-link');
                    link2.innerText = card.name2;
                    listItem2.appendChild(link2);
                    listGroup.appendChild(listItem2);

                    cardDiv.appendChild(listGroup);

                    // 將卡片加入到輪播項目中
                    carouselItem.appendChild(cardDiv);
                    carouselInner.appendChild(carouselItem);
                });

                // 將輪播項目加入到輪播容器中
                carouselContainer.appendChild(carouselInner);

                // 只有在 productCards 長度大於 1 時，才顯示控制按鈕
                if (productCards.length > 1) {
                    // 輪播控制按鈕
                    const prevButton = document.createElement('button');
                    prevButton.classList.add('carousel-control-prev');
                    prevButton.type = 'button';
                    prevButton.setAttribute('data-bs-target', '#cardCarousel');
                    prevButton.setAttribute('data-bs-slide', 'prev');
                    prevButton.innerHTML = `
                        <span class="carousel-control-prev-icon" aria-hidden="true"></span>
                        <span class="visually-hidden">Previous</span>`;
                    carouselContainer.appendChild(prevButton);

                    const nextButton = document.createElement('button');
                    nextButton.classList.add('carousel-control-next');
                    nextButton.type = 'button';
                    nextButton.setAttribute('data-bs-target', '#cardCarousel');
                    nextButton.setAttribute('data-bs-slide', 'next');
                    nextButton.innerHTML = `
                        <span class="carousel-control-next-icon" aria-hidden="true"></span>
                        <span class="visually-hidden">Next</span>`;
                    carouselContainer.appendChild(nextButton);
                }

                // 將輪播容器加入到 chatContent
                chatContent.appendChild(carouselContainer);
            } else {
                const noProductDiv = document.createElement('p');
                noProductDiv.innerText = '';
                chatContent.appendChild(noProductDiv);
            }
        })
        .catch(error => {
            console.error('Error fetching initial messages and product cards:', error);
            const errorDiv = document.createElement('div');
            errorDiv.classList.add('message', 'error');
            errorDiv.innerText = '獲取資料時發生錯誤，請稍後再試。';
            chatContent.appendChild(errorDiv);
        });
}





document.getElementById('chat-input').addEventListener('keydown', function (e) {
    if (e.key === 'Enter') {
        e.preventDefault();
        sendMessage();
    }
});
