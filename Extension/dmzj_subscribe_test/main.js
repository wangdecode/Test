function my_subscribe(type) {
    if(!isLogin){
        console.log("用户未登录");
        return;
    }
    if (type){
        my_subscribe_add(g_comic_id, 0, this);
    }else{
        my_subscribe_remove(g_comic_id, 0, this);
    }
}

function my_subscribe_add(subId,sub_type,obj) {
    var url = ajax_url+"api/subscribe/add";
    T.ajaxJsonp(url,{sub_id:subId,uid:userId,sub_type:sub_type}, function (data) {
        if(data.result==1000){
            console.log("订阅成功");
        }else{
            return 0;
        }
    })
}

function my_subscribe_remove(subId,sub_type,obj){
    var url = ajax_url+"api/subscribe/del";
    T.ajaxJsonp(url,{sub_id:subId,uid:userId,sub_type:sub_type}, function (data) {
        if(data.result==1000){
            console.log("取消订阅成功");
        }else{
            return 0;
        }
    })
}

var html ="<a>  </a><button id=\"btn1\" onclick=\"my_subscribe(true)\">订阅</button><a> </a><button id=\"btn2\" onclick=\"my_subscribe(false)\">取订</button>";
document.getElementsByClassName("h2_title2")[0].innerHTML+=html;

console.log("comic_name:", g_comic_name, "\ncomic_id:", g_comic_id);
