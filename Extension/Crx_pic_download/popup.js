//popup.html 按钮事件
document.addEventListener('DOMContentLoaded', function() {
    let obj;
    
    obj = document.getElementById('btn_start');
    obj.addEventListener('click', function() {
        start(document.getElementById('path').value);
    });
 
    obj = document.getElementById('btn_path');
    obj.addEventListener('click', function() {
        pathchange();
    });
 
    obj = document.getElementById('btn_clean');
    obj.addEventListener('click', function() {
        sendToContent({cmd:'pic_remove_space'});
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

//路径更新
function pathchange() {
    let path, pnum, spath;
    path =  document.getElementById('path').value
    pnum = path.match(/[0-9]*$/);
    if(pnum[0].length == 0) {
        spath =  path + '1';
    } else {
        spath = path.substr(0, pnum.index) + String(parseInt(pnum[0]) + 1);
    }
    let bg = chrome.extension.getBackgroundPage();
    bg.setpath(spath);
    
    document.getElementById('path').value = spath;
}
