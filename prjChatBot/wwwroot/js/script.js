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

    // 创建轮播容器
    const carouselContainer = document.createElement('div');
    carouselContainer.id = 'cardCarousel';
    carouselContainer.classList.add('carousel', 'slide');
    carouselContainer.setAttribute('data-bs-ride', 'carousel');

    const carouselInner = document.createElement('div');
    carouselInner.classList.add('carousel-inner');
    carouselContainer.appendChild(carouselInner);

    // 第一张轮播图
    const carouselItem1 = document.createElement('div');
    carouselItem1.classList.add('carousel-item', 'active');
    const card1 = document.createElement('div');
    card1.classList.add('card');
    card1.style.width = '25rem';
    card1.style.margin = 'auto';
    const img1 = document.createElement('img');
    img1.src = '/images/XiuCaiOolongTea.png';
    img1.classList.add('card-img-top');
    img1.alt = 'XiuCaiOolongTea';
    card1.appendChild(img1);
    const cardBody1 = document.createElement('div');
    cardBody1.classList.add('card-body');
    const cardTitle1 = document.createElement('h5');
    cardTitle1.classList.add('card-title');
    cardTitle1.innerText = '秀才烏龍茶';
    cardBody1.appendChild(cardTitle1);
    card1.appendChild(cardBody1);
    const listGroup1 = document.createElement('ul');
    listGroup1.classList.add('list-group', 'list-group-flush');
    const listItem1a = document.createElement('li');
    listItem1a.classList.add('list-group-item');
    const link1a = document.createElement('a');
    link1a.href = 'https://taoyuanchoice.tycg.gov.tw/Home/ProductDetail/d241036c-8095-4dc9-a7ba-0e443d0b373b#gsc.tab=0';
    link1a.classList.add('card-link');
    link1a.innerText = '產品資訊';
    link1a.target = '_blank';
    listItem1a.appendChild(link1a);
    const listItem1b = document.createElement('li');
    listItem1b.classList.add('list-group-item');
    const link1b = document.createElement('a');
    link1b.href = 'https://shopee.tw/%E3%80%90%E5%8F%8C%E9%9C%96%E8%8C%B6%E8%8E%8A%E3%80%91%E7%A7%80%E6%89%8D%E7%83%8F%E9%BE%8D%E8%8C%B6-%F0%9F%94%A5%E9%BE%8D%E7%9C%BC%E6%9C%A8%E6%85%A2%E7%83%98%F0%9F%94%A5-%E9%A6%99%E6%B0%A3%E5%AE%9C%E4%BA%BA-i.4010690.4810134291?sp_atk=d9990eb6-6b8a-467f-bd09-cdf4f0073305&xptdk=d9990eb6-6b8a-467f-bd09-cdf4f0073305';
    link1b.classList.add('card-link');
    link1b.innerText = '購買連結';
    link1b.target = '_blank';
    listItem1b.appendChild(link1b);
    listGroup1.appendChild(listItem1a);
    listGroup1.appendChild(listItem1b);
    card1.appendChild(listGroup1);
    carouselItem1.appendChild(card1);
    carouselInner.appendChild(carouselItem1);

    // 第二张轮播图
    const carouselItem2 = document.createElement('div');
    carouselItem2.classList.add('carousel-item');
    const card2 = document.createElement('div');
    card2.classList.add('card');
    card2.style.width = '25rem';
    card2.style.margin = 'auto';
    const img2 = document.createElement('img');
    img2.src = '/images/CharocoalRoasstedBlackTea.png';
    img2.classList.add('card-img-top');
    img2.alt = 'CharocoalRoasstedBlackTea';
    card2.appendChild(img2);
    const cardBody2 = document.createElement('div');
    cardBody2.classList.add('card-body');
    const cardTitle2 = document.createElement('h5');
    cardTitle2.classList.add('card-title');
    cardTitle2.innerText = '碳花紅茶';
    cardBody2.appendChild(cardTitle2);
    card2.appendChild(cardBody2);
    const listGroup2 = document.createElement('ul');
    listGroup2.classList.add('list-group', 'list-group-flush');
    const listItem2a = document.createElement('li');
    listItem2a.classList.add('list-group-item');
    const link2a = document.createElement('a');
    link2a.href = 'https://taoyuanchoice.tycg.gov.tw/Home/ProductDetail/86c48593-71da-4eda-8152-923a18ce74bc#gsc.tab=0';
    link2a.classList.add('card-link');
    link2a.innerText = '產品資訊';
    link2a.target = '_blank';
    listItem2a.appendChild(link2a);
    const listItem2b = document.createElement('li');
    listItem2b.classList.add('list-group-item');
    const link2b = document.createElement('a');
    link2b.href = 'https://shopee.tw/-%E5%8F%8C%E9%9C%96%E8%8C%B6%E8%8E%8A-%F0%9F%94%A5%E7%A2%B3%E8%8A%B1%E7%B4%85%E8%8C%B6%F0%9F%94%A5(%E4%B9%85%E6%B3%A1%E4%B8%8D%E8%8B%A6%E6%BE%80%EF%BC%8C%E5%8F%A3%E6%84%9F%E6%BF%83%E9%9F%BB)-%E5%8D%81%E5%A4%A7%E4%BC%B4%E6%89%8B%E7%A6%AE-%E5%8F%B0%E7%81%A3%E7%B4%85%E8%8C%B6-%E5%9C%A8%E5%9C%B0%E9%A2%A8%E5%91%B3%E8%8C%B6-i.4010690.25265500225?xptdk=5048ca95-b95f-4340-9266-b397a334b3ac';
    link2b.classList.add('card-link');
    link2b.innerText = '購買連結';
    link2b.target = '_blank';
    listItem2b.appendChild(link2b);
    listGroup2.appendChild(listItem2a);
    listGroup2.appendChild(listItem2b);
    card2.appendChild(listGroup2);
    carouselItem2.appendChild(card2);
    carouselInner.appendChild(carouselItem2);

    // 第三张轮播图
    const carouselItem3 = document.createElement('div');
    carouselItem3.classList.add('carousel-item');
    const card3 = document.createElement('div');
    card3.classList.add('card');
    card3.style.width = '25rem';
    card3.style.margin = 'auto';
    const img3 = document.createElement('img');
    img3.src = '/images/XiuCaiBlackTea.png';
    img3.classList.add('card-img-top');
    img3.alt = 'XiuCaiBlackTea';
    card3.appendChild(img3);
    const cardBody3 = document.createElement('div');
    cardBody3.classList.add('card-body');
    const cardTitle3 = document.createElement('h5');
    cardTitle3.classList.add('card-title');
    cardTitle3.innerText = '秀才紅茶';
    cardBody3.appendChild(cardTitle3);
    card3.appendChild(cardBody3);
    const listGroup3 = document.createElement('ul');
    listGroup3.classList.add('list-group', 'list-group-flush');
    const listItem3a = document.createElement('li');
    listItem3a.classList.add('list-group-item');
    const link3a = document.createElement('a');
    link3a.href = 'https://taoyuanchoice.tycg.gov.tw/Home/ProductDetail/4c3b708b-6d2f-4280-b01e-a964eb698a73#gsc.tab=0';
    link3a.classList.add('card-link');
    link3a.innerText = '產品資訊';
    link3a.target = '_blank';
    listItem3a.appendChild(link3a);
    const listItem3b = document.createElement('li');
    listItem3b.classList.add('list-group-item');
    const link3b = document.createElement('a');
    link3b.href = 'https://shopee.tw/-%E5%8F%8C%E9%9C%96%E8%8C%B6%E8%8E%8A-2023-%E6%98%A5%E8%8C%B6%F0%9F%94%A5%E7%A7%80%E6%89%8D%E7%B4%85%E8%8C%B6%F0%9F%94%A5(%E4%B9%85%E6%B3%A1%E4%B8%8D%E8%8B%A6%E6%BE%80)-%E5%8D%81%E5%A4%A7%E4%BC%B4%E6%89%8B%E7%A6%AE-%E5%8F%B0%E7%81%A3%E7%B4%85%E8%8C%B6-%E5%9C%A8%E5%9C%B0%E9%A2%A8%E5%91%B3%E8%8C%B6-i.4010690.23045180005?xptdk=51150d59-eda2-4b0f-9e63-61793dbf180d';
    link3b.classList.add('card-link');
    link3b.innerText = '購買連結';
    link3b.target = '_blank';
    listItem3b.appendChild(link3b);
    listGroup3.appendChild(listItem3a);
    listGroup3.appendChild(listItem3b);
    card3.appendChild(listGroup3);
    carouselItem3.appendChild(card3);
    carouselInner.appendChild(carouselItem3);

    // 添加轮播控制按钮
    const prevButton = document.createElement('button');
    prevButton.classList.add('carousel-control-prev');
    prevButton.setAttribute('type', 'button');
    prevButton.setAttribute('data-bs-target', '#cardCarousel');
    prevButton.setAttribute('data-bs-slide', 'prev');
    const prevIcon = document.createElement('span');
    prevIcon.classList.add('carousel-control-prev-icon');
    prevIcon.setAttribute('aria-hidden', 'true');
    const prevText = document.createElement('span');
    prevText.classList.add('visually-hidden');
    prevText.innerText = 'Previous';
    prevButton.appendChild(prevIcon);
    prevButton.appendChild(prevText);
    carouselContainer.appendChild(prevButton);

    const nextButton = document.createElement('button');
    nextButton.classList.add('carousel-control-next');
    nextButton.setAttribute('type', 'button');
    nextButton.setAttribute('data-bs-target', '#cardCarousel');
    nextButton.setAttribute('data-bs-slide', 'next');
    const nextIcon = document.createElement('span');
    nextIcon.classList.add('carousel-control-next-icon');
    nextIcon.setAttribute('aria-hidden', 'true');
    const nextText = document.createElement('span');
    nextText.classList.add('visually-hidden');
    nextText.innerText = 'Next';
    nextButton.appendChild(nextIcon);
    nextButton.appendChild(nextText);
    carouselContainer.appendChild(nextButton);

    chatContent.appendChild(carouselContainer);
}

document.getElementById('chat-input').addEventListener('keydown', function (e) {
    if (e.key === 'Enter') {
        e.preventDefault();
        sendMessage();
    }
});
