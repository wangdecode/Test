//监听广播
chrome.runtime.onMessage.addListener(
    function(request, sender, sendResponse) {
        if (request.message === "activate_icon") {
            chrome.pageAction.show(sender.tab.id);//激活popup
        }
        
        if (request.message === "download") {
            download(request.urls, request.path);
        }
    }
);

//下载
//urls:链接串集合    path:保存目录
function download(urls, path) {
    if (path == '\\') return;
    for (i=0;i<urls.length;i++) {
        let ext = urls[i].substr(urls[i].lastIndexOf('.'));
        if (ext.length < 4) ext = '.jpg';
        let filename = path + String(i) + ext;
        chrome.downloads.download({
            conflictAction: 'uniquify',
            method: "GET",
            saveAs: false,
            url: urls[i],
            filename: filename
        });
    }
}

let bgpath; //保存更新后的目录
function getpath() {
    return bgpath;
}

function setpath(path) {
    bgpath = path;
}
