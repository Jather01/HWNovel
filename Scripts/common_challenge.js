function navbarHighlight() {
    $(".m3").addClass("on sub");

    var pageName = window.location.pathname.slice(-3);

    var html = "";
    html += "<ul class='snb'>"
    html += "    <li class='" + (pageName == "Rom" ? "on" : "") + "'><a href='/Challenge/Rom' class='sub_link'><span class='sub_txt'>로맨스</span></a></li>";
    html += "    <li class='" + (pageName == "Rof" ? "on" : "") + "'><a href='/Challenge/Rof' class='sub_link'><span class='sub_txt'>로판</span></a></li>";
    html += "    <li class='" + (pageName == "Fan" ? "on" : "") + "'><a href='/Challenge/Fan' class='sub_link'><span class='sub_txt'>판타지</span></a></li>";
    html += "    <li class='" + (pageName == "Hro" ? "on" : "") + "'><a href='/Challenge/Hro' class='sub_link'><span class='sub_txt'>무협</span></a></li>";
    html += "    <li class='" + (pageName == "Mys" ? "on" : "") + "'><a href='/Challenge/Mys' class='sub_link'><span class='sub_txt'>미스터리</span></a></li>";
    html += "</ul>";

    $(".m3").append(html);
}