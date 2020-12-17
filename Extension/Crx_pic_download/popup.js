//popup.html 按钮事件
document.addEventListener('DOMContentLoaded', function() {
    let obj;
    obj = document.getElementById('btn_start');
    obj.addEventListener('click', function() {
        start(document.getElementById('path').value);
    });
    
    obj = document.getElementById('path');
    obj.addEventListener('change', function() {
        let bg = chrome.extension.getBackgroundPage();
        bg.setpath(document.getElementById('path').value);
    });
    
    init();
});

//监听广播
chrome.runtime.onMessage.addListener(
    function(request, sender, sendResponse) {
        if (request.message === "pic_num") {
            document.getElementById('pic_num').innerHTML = request.num;
        }
    }
);

//发送广播到Content.js
function sendToContent(message) {
    chrome.tabs.query({active: true, currentWindow: true}, function(tabs) {
        chrome.tabs.sendMessage(tabs[0].id, message);
    });
};

//开始下载
function start(path) {
    pathchange(path);
    if (path[path.length-1] != "\\")
        path = path + "\\";
    
    sendToContent({cmd:'start' , path: path});
}

//初始化
function init() {
    let bg = chrome.extension.getBackgroundPage();
    let path = bg.getpath();
    if (path == null) path = "";
    document.getElementById('path').value = path;
    
    sendToContent({cmd:'getnum'});
}

//路径自动更新
function pathchange(path) {
    let pnum, num, spath;
    pnum = path.match(/[0-9]*$/);
    if(pnum[0].length == 0)
        spath =  path + '1';
    else
    {
        num = parseInt(pnum[0]) + 1;
        spath = path.substr(0, pnum.index) + String(num);
    }
    let bg = chrome.extension.getBackgroundPage();
    bg.setpath(spath);
}
