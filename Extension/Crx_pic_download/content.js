//发消息，激活popup
chrome.runtime.sendMessage({"message": "activate_icon"});

//监听广播
chrome.runtime.onMessage.addListener(function(request, sender, sendResponse) {
    if(request.cmd == 'start') {
        let path = request.path;
        let urls = pic_explain();
        //console.log(path, urls);
        chrome.runtime.sendMessage({
            "message": "download", "urls": urls, "path": path});
    }
    if(request.cmd == 'pic_remove_space') {
        pic_remove_space();
    }
    if(request.cmd == 'getnum') {
        let num = pic_explain().length;
        chrome.runtime.sendMessage({
            "message": "pic_num", "num": num});
    }
});

//图片解析
function pic_explain() {
    //let main = document.getElementById("");
    let pics = document.getElementsByTagName("img");
    if (pics.length == 0) return;
    
    let pics_url = new Array();
    let index = 0;
    for (i=0;i<pics.length;i++) {
        //判断图片高, 宽
        if (pics[i].height < 300 || pics[i].width < 100) continue;
        pics_url[index]=pics[i].src;
        index++;
    }
    
    return pics_url;
}

//图片去除间隔
function pic_remove_space() {
    if(window.location.hostname.indexOf("cnanjie.com") == -1 )
        return;
    let a=document.getElementsByClassName("img_info");
    for(i=a.length-1;i>=0;i--) {
        a[i].remove();
    }
}
