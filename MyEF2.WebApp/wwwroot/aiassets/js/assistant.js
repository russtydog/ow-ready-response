// trevor.js
function copyText(clickedElement, caller) {
    // Get the text from the clickedElement
    var text = document.getElementById(clickedElement).innerText;

    // Copy the text to the clipboard
    navigator.clipboard.writeText(text);

    //set the caller class to "fas fa-check" for 2 seconds
    caller.className = "fas fa-check trevcopy";
    setTimeout(function () {
        caller.className = "far fa-copy trevcopy";
    }, 2000);


}


function generateGUID() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0,
            v = c === 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}

// Check if the API key is provided
if (!apiKey) {
    // If 'apiKey' is missing, throw an error and terminate script execution
    throw new Error("API key is missing. Please provide the 'apiKey' parameter.");
}

var ConversationId;
var historyLoded = false;


document.addEventListener("DOMContentLoaded", function () {


    // CSS file references
    var cssLinks = [
        "[[ApplicationURL]]/aiassets/css/cb-style.css",
        "[[ApplicationURL]]/aiassets/css/rubik.css",
        "[[ApplicationURL]]/aiassets/css/assistant.css",
    ];

    // Load CSS files
    cssLinks.forEach(function (link) {
        var cssLink = document.createElement("link");
        cssLink.href = link;
        cssLink.rel = "stylesheet";
        document.head.appendChild(cssLink);
    });

    // Load JavaScript files
    function loadScript(url, callback) {
        var script = document.createElement("script");
        script.src = url;
        script.onload = callback; // Execute callback function when script is loaded
        document.head.appendChild(script);
    }

    var jsFiles = [
        "[[ApplicationURL]]/aiassets/js/jquery-3.6.0.js",
        "[[ApplicationURL]]/aiassets/js/bootstrap.min.js",
        "[[ApplicationURL]]/aiassets/js/jquery-ui.js",
        "[[ApplicationURL]]/aiassets/js/jquery.bootstrap.js",
        "[[ApplicationURL]]/aiassets/js/jquery.min.js"

    ];

    jsFiles.forEach(loadScript);

    // Create and render HTML elements
    var chatBox = document.createElement("div");
    chatBox.className = "chat-box";
    chatBox.style.display = "none";


    var chatBoxHeader = document.createElement("div");
    chatBoxHeader.className = "chat-box-header";
    var h3 = document.createElement("h3");
    h3.textContent = "[[ChatbotName]]";
    h3.style.color = "[[PrimaryColour]]";
    h3.style.fontFamily = "'Source Sans Pro', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif, 'Apple Color Emoji', 'Segoe UI Emoji', 'Segoe UI Symbol'";
    var headerBtnSet = document.createElement("div");
    headerBtnSet.className = "header-btn-set";
    var refreshBtn = document.createElement("i");
    refreshBtn.className = "fas fa-sync header-btn";
    refreshBtn.id = "cb-refresh";
    refreshBtn.style.color = "black";
    refreshBtn.style.fontSize = "16px";
    var maximizeBtn = document.createElement("i");
    maximizeBtn.className = "fa fa-window-maximize header-btn";
    maximizeBtn.id = "cb-maximize";
    maximizeBtn.style.color = "black";
    var closeBtn = document.createElement("i");
    closeBtn.className = "fa fa-times header-btn";
    closeBtn.id = "cb-close";
    closeBtn.style.color = "black";
    headerBtnSet.appendChild(refreshBtn);

    if (window.innerWidth > 300) {
        headerBtnSet.appendChild(maximizeBtn);
    }
    headerBtnSet.appendChild(closeBtn);
    chatBoxHeader.appendChild(h3);
    chatBoxHeader.appendChild(headerBtnSet);



    var messagesDiv = document.createElement("div");
    messagesDiv.className = "messages";
    var messagesContainer = document.createElement("div");
    messagesContainer.id = "messages";
    var divTyping = document.createElement("div");
    divTyping.id = "divTyping";
    divTyping.style.display = "none";
    divTyping.style.padding = "0px";
    //divTyping.style.width = "50px";
    divTyping.className = ""; //was trevormessages
    var typingImage = document.createElement("img");
    typingImage.src = "[[ApplicationURL]]/aiassets/img/typing.gif";
    typingImage.id = "imgTyping";
    typingImage.style.height = "50px";
    divTyping.appendChild(typingImage);
    messagesDiv.appendChild(messagesContainer);
    messagesDiv.appendChild(divTyping);

    var chatBoxFaq = document.createElement("div");
    chatBoxFaq.className = "chat-box-faq";

    var inputContainer = document.createElement("div");
    inputContainer.id = "input-container";
    var inputGroup = document.createElement("div");
    inputGroup.className = "input-group";
    var userMessageTextarea = document.createElement("textarea");
    userMessageTextarea.id = "userMessage";
    userMessageTextarea.className = "user-message";
    userMessageTextarea.style.outline = "none";
    userMessageTextarea.style.overflow = "hidden";
    userMessageTextarea.style.resize = "none";
    userMessageTextarea.rows = "1";
    userMessageTextarea.cols = "50";
    userMessageTextarea.placeholder = "Type your message...";

    var inputGroupAppend = document.createElement("div");
    inputGroupAppend.className = "input-group-append";
    var sendButton = document.createElement("button");
    sendButton.className = "btn btn-primary";
    sendButton.style.backgroundColor = "[[PrimaryColour]]";
    sendButton.style.minWidth = "10px";
    sendButton.style.height = "Auto";
    sendButton.onclick = function () {
        callAjaxFunction(false);  // Passing 'false' as a parameter
    };
    var sendIcon = document.createElement("i");
    sendIcon.className = "fas fa-paper-plane";
    sendButton.appendChild(sendIcon);
    inputGroupAppend.appendChild(sendButton);
    inputGroup.appendChild(userMessageTextarea);
    inputGroup.appendChild(inputGroupAppend);
    inputContainer.appendChild(inputGroup);

    chatBox.appendChild(chatBoxHeader);
    //chatBox.appendChild(optionDiv);
    chatBox.appendChild(messagesDiv);
    chatBox.appendChild(chatBoxFaq);
    chatBox.appendChild(inputContainer);

    var cbPosition = document.createElement("div");
    cbPosition.className = "cb-position";
    cbPosition.id = "draggable";
    var chatButton = document.createElement("div");
    chatButton.className = "chat-button";
    var assistantIcon = document.createElement("i");
    assistantIcon.className = "fa fa-comments assistant-icon";
    var label = document.createElement("label");
    label.style.cursor = "pointer";
    label.textContent = "[[ChatbotName]]";
    chatButton.appendChild(assistantIcon);
    chatButton.appendChild(label);
    cbPosition.appendChild(chatButton);

    document.body.appendChild(chatBox);
    document.body.appendChild(cbPosition);

    var cbMaximized = false;

    function copyText(clickedElement) {
        // Get the text from the div
        var text = clickedElement.parentNode.innerText;

        // Copy the text to the clipboard
        navigator.clipboard.writeText(text);
    }

    // Your JavaScript code here
    function callAjaxFunction(ignoreContent) {
        // Add your AJAX function here
        $("#divTyping").show();
        var userMessage = $("#userMessage").val();
        $("#userMessage").val("");

        $("#messages").append("<p class=mymessages>" + userMessage + "</p>");

        scrollToBottom();

        //create a ConversationId as a guid
        if (localStorage.getItem('EFConversationId') === null) {
            ConversationId = generateGUID();
            localStorage.setItem('EFConversationId', ConversationId);
        } else {
            ConversationId = localStorage.getItem('EFConversationId');
        }


        $.ajax({
            url: "[[ApplicationURL]]/AI/Chat?handler=[[HandlerName]]",
            method: "POST",
            headers: {
                "X-Api-Key": "your_valid_api_key"
            },
            contentType: "application/json",
            // data: JSON.stringify($("#messages").html()),
            data: JSON.stringify({
                messages: $("#messages").text(),
                selectedOption: 'documents', //always documents
                aPiKey: apiKey,
                conversationId: ConversationId,
                lastMessage: userMessage,
                ignoreContent: ignoreContent
            }),
            success: function (startResponse) {

                //start a timer and within the for loop get the message stream until StreamStatus=="Complete"

                var messageId = startResponse.messageId;
                console.log("Succesfully started" + messageId);
                var pollInterval = setInterval(function () {
                    $.ajax({
                        url: "[[ApplicationURL]]/AI/Chat/?handler=MessageStream&MessageId=" + startResponse.messageId, // Replace with your actual URL
                        method: 'POST',
                        data: JSON.stringify({
                            messageId: startResponse.messageId,
                        }),
                        success: function (response) {
                            console.log("Polling stream status: " + response.streamStatus);

                            if (typeof response.messageStream !== 'undefined') {
                                $("#divTyping").hide();
                            }

                            //if div of id=response.messageId doesn't exist, then create it, else update the text
                            if ($("#" + startResponse.messageId).length) {
                                $("#" + startResponse.messageId).html(response.messageStream);
                            } else {
                                if (typeof response.messageStream !== 'undefined') {
                                    $("#messages").append("<div class=trevormessages id='" + startResponse.messageId + "'>" + response.messageStream + "<i class='far fa-copy trevcopy' onclick='copyText(\"" + startResponse.messageId + "\",this)'></i></div>");
                                }
                            }

                            scrollToBottom();

                            if (response.streamStatus === 'Complete') {
                                // If the task is complete, stop polling and redirect to the file
                                clearInterval(pollInterval);
                                console.log("Stream is complete");
                                console.log(response);
                                //$("#" + startResponse.messageId).html(htmlDecode(response.finalResponse) + "<i class='far fa-copy trevcopy' onclick='copyText(\"" + startResponse.messageId + "\",this)'></i>");
                                $("#" + startResponse.messageId).html(response.messageStream + "<i class='far fa-copy trevcopy' onclick='copyText(\"" + startResponse.messageId + "\",this)'></i>");

                            }
                        }
                    });
                }, 100);

                //$("#messages").append("<div class=trevormessages id='" + response.messageId + "'>" + response.message + "<i class='far fa-copy trevcopy' onclick='copyText(\"" + response.messageId + "\",this)'></i></div>");
                //$('.chat-button').trigger('click') //opens chat window

            },
            error: function (error) {
                // $("#messages").append("<p class=trevormessages>Apologies, but Trevor is having a problem connecting to your Procurement Policy. Please try again later.</p>");
                $("#messages").append("<p class=trevormessages>Apologies, but OpenAI is having a problem connecting to your Documents or is offline. Please try again later.</p>");
                $("#divTyping").hide();
            }
        });

        var textarea = document.getElementById('userMessage');
        textarea.style.height = 'auto';
    }

    function htmlDecode(input) {
        var doc = new DOMParser().parseFromString(input, "text/html");
        return doc.documentElement.textContent;
    }

    function scrollToBottom() {
        // Add your scroll function here
        var messagesDiv = $('.messages');

        messagesDiv.scrollTop(messagesDiv.prop('scrollHeight'));
    }



    function autoResize() {
        // Add your auto resize function here
        this.style.height = 'auto';
        this.style.height = this.scrollHeight + 'px';
    }
    $(document).ready(function () {
        var userMessageInput = document.getElementById("userMessage");
        userMessageInput.addEventListener("keypress", function (event) {
            if (event.keyCode === 13) {
                event.preventDefault();
                callAjaxFunction(false);
            }
        });


        $("#sendMessage").click(function () {
            callAjaxFunction(false);
        });

        var textarea = document.getElementById('userMessage');
        textarea.addEventListener('input', autoResize, false);

        function autoResize() {
            this.style.height = 'auto';
            this.style.height = this.scrollHeight + 'px';
        }


    });

    jsFiles.forEach(function (url) {
        loadScript(url, function () {


            $(".chat-button").draggable().click(function () {
                $('.chat-button').hide();
                $('.chat-box').show();

                if (historyLoded === false) {
                    if (localStorage.getItem('EFConversationId') != null) {
                        //means we have an existing id (not a new user) so load the conversation
                        ConversationId = localStorage.getItem('EFConversationId');

                        $.ajax({
                            url: "[[ApplicationURL]]/AI/Chat?handler=History",
                            method: "POST",
                            headers: {
                                "X-Api-Key": "your_valid_api_key"
                            },
                            contentType: "application/json",
                            // data: JSON.stringify($("#messages").html()),
                            data: JSON.stringify({
                                conversationId: ConversationId,
                                aPiKey: apiKey
                            }),
                            success: function (response) {
                                //foreach response, append to messages
                                console.log(response);
                                if (response !== null) {
                                    response.response.$values.forEach(item => {
                                        if (item.class === "trevormessages") {
                                            $("#messages").append("<div class=trevormessages id='" + item.messageId + "'>" + item.message + "<i class='far fa-copy trevcopy' onclick='copyText(\"" + item.messageId + "\",this)'></i></div>");

                                        }
                                        else {
                                            $("#messages").append('<div class="' + item.class + '">' + item.message + '</div>');
                                        }

                                    });
                                }

                                scrollToBottom();
                                //$('.chat-button').trigger('click') //opens chat window

                            },
                            error: function (error) {
                                // $("#messages").append("<p class=trevormessages>Apologies, but Trevor is having a problem connecting to your Procurement Policy. Please try again later.</p>");
                                $("#messages").append("<p class=trevormessages>Apologies, but OpenAI is having a problem connecting to your Documents or is offline. Please try again later.</p>");
                                $("#divTyping").hide();
                            }
                        });


                        //$("#messages").append("<p class=hiddenprompt>Please introduce yourself and ask if you can be of any assistance.</p>");
                        //callAjaxFunction(true);
                    }
                    else {
                        //this is new so initiate
                        $("#messages").append("<p class=hiddenprompt>Please introduce yourself and ask if you can be of any assistance.</p>");
                        callAjaxFunction(true);
                    }

                    historyLoded = true;
                }
                //}

            });

            function htmlDecode(input) {
                var doc = new DOMParser().parseFromString(input, "text/html");
                return doc.documentElement.textContent;
            }

            $('#cb-close').on('click', function () {
                $('.chat-button').show();
                $('.chat-box').hide();
            });
            $('#cb-refresh').on('click', function () {

                localStorage.setItem('EFConversationId', generateGUID());
                $("#messages").html("");
                $("#messages").append("<p class=hiddenprompt>Please introduce yourself and ask if you can be of any assistance.</p>");

                callAjaxFunction(true);
            });
            $('#cb-maximize').on('click', function () {

                /*alert(cbMaximized);*/
                if (cbMaximized === "true") {
                    $('.chat-box').css({
                        "width": "",
                        "height": ""
                    });
                    cbMaximized = "false";
                }
                else {
                    $('.chat-box').css({
                        "width": "calc(100% - 270px)",
                        "height": "calc(100% - 135px)"
                    });
                    cbMaximized = "true";
                }
            });

        });
    });
});
