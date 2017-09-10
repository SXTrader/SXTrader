var headtime=new Date();
function getCookie(name){
	var cname = name + "=";
	var dc = document.cookie;
	if (dc.length > 0){
		begin = dc.indexOf(cname);
		if (begin != -1){
			begin += cname.length;
			end = dc.indexOf(";", begin);
			if (end == -1) end = dc.length;
			return dc.substring(begin, end);
		}
	}
	return null;
}
function writeCookie(name, value) 
{ 
	var expire = ""; 
	var hours = 365;
	expire = new Date((new Date()).getTime() + hours * 3600000); 
	expire = ";path=/;expires=" + expire.toGMTString(); 
	document.cookie = name + "=" + escape(value) + expire; 
}

if(window.location.href.toLowerCase().indexOf("analysis")>0) {  //analysis page,soccer,basketball
    var css=getCookie("css");
    if(css==null) css="blue";
    document.write('<link href="/style/' + css + '.css" type="text/css" rel="stylesheet"  />')
}

function MM_findObj(n, d) { //v4.01
  var p,i,x;  if(!d) d=document; if((p=n.indexOf("?"))>0&&parent.frames.length) {
    d=parent.frames[n.substring(p+1)].document; n=n.substring(0,p);}
  if(!(x=d[n])&&d.all) x=d.all[n]; for (i=0;!x&&i<d.forms.length;i++) x=d.forms[i][n];
  for(i=0;!x&&d.layers&&i<d.layers.length;i++) x=MM_findObj(n,d.layers[i].document);
  if(!x && d.getElementById) x=d.getElementById(n); return x;
}

function MM_showHideLayers() { //v6.0
  var i,p,v,obj,args=MM_showHideLayers.arguments;
  for (i=0; i<(args.length-2); i+=3) if ((obj=MM_findObj(args[i]))!=null) { v=args[i+2];
    if (obj.style) { obj=obj.style; v=(v=='show')?'visible':(v=='hide')?'hidden':v; }
    obj.visibility=v; }
}

function getIENumber() {
    var ieNum = 0;
    if (document.all)
        ieNum = navigator.userAgent.toString().toLowerCase().match(/msie ([\d.]+)/)[1];

    return ieNum;
}

var startani_C,startani_A,startani_B,pop_TC;
var oPopup;
try{ oPopup=window.createPopup();}
catch(e){}

function ShowCHWindow(str,matchnum){
	imagewidth=460;
	imageheight=28+27*matchnum ;
	var st="<table width=460 border=0 cellpadding=0 cellspacing=0 style='border: 3px solid #090;background-color: #FFF;'>";
	st=st + "<tr style='background-color: #DBECA6;'><td height=22 colspan=6><SPAN style='margin-left:6px'><B>NowGoal.com</B></SPAN></td></tr>";
	st=st + str;
		st=st + "</table>";  
	st=st + "<style type='text/css'>";
	st=st + "td { font-family: 'Arial';font-size: 12px;}";
	st=st + ".line td { border-bottom:solid 1px #FFD8CA; line-height:26px;font-size: 13px;}";
	st=st + "</style>";
	x=280;
	x=(screen.width-imagewidth)/2;
	y=1;
	oPopupBody = oPopup.document.body;
	oPopupBody.innerHTML = st;
	oPopupBody.style.cursor="pointer";
		oPopupBody.title = "Hit to close";
	oPopupBody.onclick=dismisspopup;
	oPopupBody.oncontextmenu=dismisspopup;
	pop_TC=40;
	pop();
}


function pop(){
  try{
	oPopup.show(x,y,imagewidth, imageheight);
	startani_A=setTimeout("pop()",300);  //显示15秒
	if(pop_TC<0){dismisspopup();};
	pop_TC=pop_TC-1;
  }catch(e){}
}
function dismisspopup(){
	clearTimeout(startani_A);
	oPopup.hide();
}

var goal=0,goalTime;
function ShowCHWindow123(str,matchnum){
	var st="<table width=460 border=0 cellpadding=0 cellspacing=0 style='border: 3px solid #090;background-color: #FFF;'>";
	st=st + "<tr style='background-color: #DBECA6;'><td height=22 colspan=6><SPAN style='margin-left:6px'><B>NowGoal.com</B></SPAN></td></tr>";
	st=st + str;
		st=st + "</table>";  
	st=st + "<style type='text/css'>";
	st=st + "td { font-family: 'Arial';font-size: 12px;}";
	st=st + ".line td { border-bottom:solid 1px #FFD8CA; line-height:26px;font-size: 13px;}";
	st=st + "</style>";
	document.getElementById("div_goal").innerHTML=st;
	document.getElementById("div_goal").style.display="";
	document.getElementById('div_goal').style.left =(document.body.clientWidth/2-175) +"px";
	goal=20;
	showTime();
	
}

function showTime()
{
    if(goal< 0)
    {
	   document.getElementById("div_goal").innerHTML="";
	   document.getElementById("div_goal").style.display="none";
	   window.clearTimeout(goalTime);
    }
    goal-=1;
	goalTime=window.setTimeout("showTime()",1000);
}

function showgoallist(ID){
	window.open("/detail/" + ID +".html", "","scrollbars=yes,resizable=yes,width=668, height=450,top=50");
}
function thai_showgoallist(ID){
	window.open("/thai/detail/" + ID +".html", "","scrollbars=yes,resizable=yes,width=668, height=450,top=50");
}
function  analysis(ID){
	window.open("/analysis/" + ID +".html");
}
function  thai_analysis(ID){
	window.open("http://data.nowgoal.com/thai/analysis/" + ID +".html");
}
function Odds(ID){
	var theURL="http://data.nowgoal.com/OddsComp.aspx?id="  +ID;
	window.open(theURL);
}
function thai_Odds(ID){
	var theURL="http://data.nowgoal.com/thai/OddsComp.aspx?id="  +ID;
	window.open(theURL);
}
function odds1x2(ID){
	window.open("/1x2/"  +ID+".htm");
}
function thai_odds1x2(ID){
	window.open("/thai/1x2/"  +ID+".htm");
}
function Team(ID){
	window.open("http://info.nowgoal.com/en/team/summary.aspx?TeamID="+ID);
}
function thai_Team(ID){
	window.open("http://info.nowgoal.com/tai/team/summary.aspx?TeamID="+ID );
}

var zXml = {
    useActiveX: (typeof ActiveXObject != "undefined"),
    useXmlHttp: (typeof XMLHttpRequest != "undefined")
};
zXml.ARR_XMLHTTP_VERS = ["Msxml2.XMLHTTP","Microsoft.XMLHTTP"];
function zXmlHttp() {}
zXmlHttp.createRequest = function ()
{
    if (zXml.useXmlHttp)  return new XMLHttpRequest(); 
    if(zXml.useActiveX) 
    {  
        if (!zXml.XMLHTTP_VER) {
            for (var i=0; i < zXml.ARR_XMLHTTP_VERS.length; i++) {
                try {
                    new ActiveXObject(zXml.ARR_XMLHTTP_VERS[i]);
                    zXml.XMLHTTP_VER = zXml.ARR_XMLHTTP_VERS[i];
                    break;
                } catch (oError) {}
            }
        }        
        if (zXml.XMLHTTP_VER) return new ActiveXObject(zXml.XMLHTTP_VER);
    } 
    alert("Sorry，XML object unsupported by your computer,please setup XML object or change explorer.");
};

var week= new Array("Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday");

function formatDate(t,type)
{
    var strTime="";
    var t1 = t.split(",");
	var t2 = new Date(t1[0],t1[1],t1[2],t1[3],t1[4],t1[5]);
	t2 = new Date(Date.UTC(t2.getFullYear(),t2.getMonth(),t2.getDate(),t2.getHours(),t2.getMinutes(),t2.getSeconds())); 

	if(type==1)
	{  
	   strTime=((t2.getMonth()+1) +"/" + t2.getDate() +"/" + t2.getFullYear() +"&nbsp;"+ formatTime2(t2) + "&nbsp;" + week[t2.getDay()]);
    }
	else if(type==2)
	{
	  strTime=((t2.getMonth()+1) +"/" + t2.getDate() +"/" + t2.getFullYear() +"&nbsp;"+ formatTime2(t2));
    }
	else 
	{
	  strTime=((t2.getMonth()+1) +"/" + t2.getDate() +"/" + t2.getFullYear());
    } 
   document.write(strTime);
}

function formatTime2(t)
{   
	var h=t.getHours();
	var m=t.getMinutes();
	var result="";
	if(h<10) h="0" + h;
	if(m<10) m="0" + m;
	return h+":"+m;
}

function showExplain(exlist, hometeam, guestteam) {
    // 广东体育; 1 | 1; 2 | 5; 12 | 90, 1 - 1; 2 - 2; 1, 2 - 2; 5 - 4; 1
    //;|1;2|;|90,3-3;2-5;2,2-2;;
     hometeam = hometeam.replace(/<[^>].*?>/g, "");
    guestteam = guestteam.replace(/<[^>].*?>/g, "");
    hometeam = hometeam.replace("(N)", "");
    guestteam = guestteam.replace("(N)", "");
     var explainList = "";
     if (exlist != "") {
         var arrExplain = exlist.split('|');
             
         if (arrExplain[1].split(';')[0] != "") {
             explainList += "kick-off(";
             if (arrExplain[1].split(';')[0] == "1")
                 explainList += hometeam + ")";
             else if (arrExplain[1].split(';')[0] == "2")
                 explainList += guestteam + ")";

         }
         if (arrExplain[1].split(';')[1] != "" && window.location.href.toLowerCase().indexOf("tv=false") == -1) {
             if (arrExplain[1].split(';')[1] == "3"|| arrExplain[1].split(';')[1] == "4"|| arrExplain[1].split(';')[1] == "5")
             {
                 if(explainList!="")
                    explainList += "<br />";
                    
                    explainList += "<a href= http://en.city007.net/ target=_blank><font color=blue>[ Live TV]</font></a>";
             }
         }
         if (arrExplain[2].split(';')[0] != "") {
         
                  if(explainList!="")
                    explainList += "<br />";
                    
             explainList += "corner(" + arrExplain[2].split(';')[0] + ") | ";
             explainList += "corner(" + arrExplain[2].split(';')[1] + ")";
         }
         var scoresList = arrExplain[3].split(';');
         if (scoresList[0] != "") {
         
                 if(explainList!="")
                    explainList += "<br />";
                    
             explainList += scoresList[0].split(',')[0] + "Min[" + scoresList[0].split(',')[1] + "],";
             if (scoresList[1] != "")
                 explainList += "two Round[" + scoresList[1] + "],";
             if (scoresList[2] != "") {
                 if (scoresList[2].split(',')[0] == "1")
                     explainList += "120Min[" + scoresList[2].split(',')[1] + "],";
                 else
                     explainList += "Ot[" + scoresList[2].split(',')[1] + "],";
             }
             if (scoresList[3] != "")
                 explainList += "Pen[" + scoresList[3] + "],";
             explainList += (scoresList[4] == "1" ? hometeam : guestteam) + " Win";
         }
     }
     

     return explainList;
 }
 
 function SetHome(type)
 {
    var varType="chenjl";
    if(document.getElementById("cbHome").checked)
        varType=type;
      
     writeCookie("setHomeType", varType);
     var oXmlHttp_ad2 = zXmlHttp.createRequest();
     oXmlHttp_ad2.open("get", "/IPSearch.aspx?setCookie="+varType+"&" + Date.parse(new Date()), false);
     oXmlHttp_ad2.send(null);
 }
 
 
 if (!window.ActiveXObject) {
     HTMLElement.prototype.insertAdjacentElement = function(where, parsedNode) {
         switch (where) {
             case "beforeBegin":
                 this.parentNode.insertBefore(parsedNode, this);
                 break;
             case "afterBegin":
                 this.insertBefore(parsedNode, this.firstChild);
                 break;
             case "beforeEnd":
                 this.appendChild(parsedNode);
                 break;
             case "afterEnd":
                 if (this.nextSibling)
                     this.parentNode.insertBefore(parsedNode, this.nextSibling);
                 else
                     this.parentNode.appendChild(parsedNode);
                 break;
         }
     }
 }
 function getTopHeight() {
     var adTop = 0;
     if (document.documentElement && document.documentElement.scrollTop)
         adTop = document.documentElement.scrollTop;
     else if (document.body)
         adTop = document.body.scrollTop
     else
         adTop = window.pageYOffset;
         
     return adTop;
 }