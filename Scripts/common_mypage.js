function navbarHighlight() {
    $(".m4").addClass("on sub");

    var pageName = window.location.pathname.slice(-3);

    var html = "";
    html += "<ul class='snb'>"
    html += "    <li class='" + (pageName == "Rec" ? "on" : "") + "'><a href='/Mypage/Rec' class='sub_link'><span class='sub_txt'>최근 본 작품</span></a></li>";
    html += "    <li class='" + (pageName == "Wis" ? "on" : "") + "'><a href='/Mypage/Wis' class='sub_link'><span class='sub_txt'>관심작품</span></a></li>";
    html += "    <li class='" + (pageName == "Cha" ? "on" : "") + "'><a href='/Mypage/Cha' class='sub_link'><span class='sub_txt'>도전 웹소설</span></a></li>";
    html += "    <li class='" + (pageName == "Min" ? "on" : "") + "'><a href='/Mypage/Min' class='sub_link'><span class='sub_txt'>내 정보</span></a></li>";
    html += "</ul>";

    $(".m4").append(html);
}