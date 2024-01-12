function navbarHighlight() {
    $(".m2").addClass("on sub");

    var pageName = window.location.pathname.slice(-3);

    var html = "";
    html += "<ul class='snb'>"
    html += "    <li class='" + (pageName == "Day" ? "on" : "") + "'><a href='/Serial/Day' class='sub_link'><span class='sub_txt'>요일별</span></a></li>";
    html += "    <li class='" + (pageName == "New" ? "on" : "") + "'><a href='/Serial/New' class='sub_link'><span class='sub_txt'>이 달의 신작</span></a></li>";
    html += "    <li class='" + (pageName == "Rom" ? "on" : "") + "'><a href='/Serial/Rom' class='sub_link'><span class='sub_txt'>로맨스</span></a></li>";
    html += "    <li class='" + (pageName == "Rof" ? "on" : "") + "'><a href='/Serial/Rof' class='sub_link'><span class='sub_txt'>로판</span></a></li>";
    html += "    <li class='" + (pageName == "Fan" ? "on" : "") + "'><a href='/Serial/Fan' class='sub_link'><span class='sub_txt'>판타지</span></a></li>";
    html += "    <li class='" + (pageName == "Hro" ? "on" : "") + "'><a href='/Serial/Hro' class='sub_link'><span class='sub_txt'>무협</span></a></li>";
    html += "    <li class='" + (pageName == "Mys" ? "on" : "") + "'><a href='/Serial/Mys' class='sub_link'><span class='sub_txt'>미스터리</span></a></li>";
    html += "    <li class='" + (pageName == "Fin" ? "on" : "") + "'><a href='/Serial/Fin' class='sub_link'><span class='sub_txt'>완결</span></a></li>";
    html += "</ul>";

    $(".m2").append(html);
}