function my_subscribe(type) {
    if(!isLogin){
        console.log("用户未登录");
        return;
    }
    if (type){
        my_subscribe_add();
    }else{
        my_subscribe_remove();
    }
}

function my_subscribe_add() {
    
}

function my_subscribe_remove(){
    
}

var html ="<a>  </a><button id=\"btn1\" onclick=\"my_subscribe(true)\">订阅</button><a> </a><button id=\"btn2\" onclick=\"my_subscribe(false)\">取订</button>";
document.getElementsByClassName("h2_title2")[0].innerHTML+=html;

console.log("comic_name:", g_comic_name, "\ncomic_id:", g_comic_id);
