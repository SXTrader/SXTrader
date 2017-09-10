<?php
/*
 * Created on 27.07.2011
 *
 * To change the template for this generated file go to
 * Window - Preferences - PHPeclipse - PHP - Code Templates
 */
 include 'LSDatabase.php';
 include 'LSHistoricMatchEvent.php';
 include 'LSHistoricMatch.php';
 include 'LSHistoricDataStatistic.php';
 
 
 
 $teamAId = isset($_GET["teamAId"]) ? $_GET["teamAId"]+0 : 0;
 $teamBId = isset($_GET["teamBId"]) ? $_GET["teamBId"]+0 : 0;
 $league = isset($_GET["league"])?$_GET["league"]: "";
 $count = isset($_GET["count"])?$_GET["count"]+0: 30;
 $age = isset($_GET["age"])?$_GET["age"]+0: 0;
 
 $statisticDb = new LSDatabase();
 $statistic = $statisticDb->getStatistics($teamAId,$teamBId, $league, $count, $age);
 //var_dump($statistic);

 header('Content-Type:text/xml; charset=UTF-8');
  $xmlString = $statistic->buildXML();
 //echo html_entity_decode($statistic->buildXML());
 //$statistic->buildXML();
 //header('Content-Type:text/xml; charset=UTF-8'); 
  $xmlString = xmlEntities($xmlString);
 echo $xmlString;
 
 
 function xmlEntities($str)
{
	$xml = array('&#34;','&#38;','&#38;','&#60;','&#62;','&#160;','&#161;','&#162;','&#163;','&#164;','&#165;','&#166;','&#167;','&#168;','&#169;','&#170;','&#171;','&#172;','&#173;','&#174;','&#175;','&#176;','&#177;','&#178;','&#179;','&#180;','&#181;','&#182;','&#183;','&#184;','&#185;','&#186;','&#187;','&#188;','&#189;','&#190;','&#191;','&#192;','&#193;','&#194;','&#195;','&#196;','&#197;','&#198;','&#199;','&#200;','&#201;','&#202;','&#203;','&#204;','&#205;','&#206;','&#207;','&#208;','&#209;','&#210;','&#211;','&#212;','&#213;','&#214;','&#215;','&#216;','&#217;','&#218;','&#219;','&#220;','&#221;','&#222;','&#223;','&#224;','&#225;','&#226;','&#227;','&#228;','&#229;','&#230;','&#231;','&#232;','&#233;','&#234;','&#235;','&#236;','&#237;','&#238;','&#239;','&#240;','&#241;','&#242;','&#243;','&#244;','&#245;','&#246;','&#247;','&#248;','&#249;','&#250;','&#251;','&#252;','&#253;','&#254;','&#255;');
	$html = array('&quot;','&amp;','&amp;','&lt;','&gt;','&nbsp;','&iexcl;','&cent;','&pound;','&curren;','&yen;','&brvbar;','&sect;','&uml;','&copy;','&ordf;','&laquo;','&not;','&shy;','&reg;','&macr;','&deg;','&plusmn;','&sup2;','&sup3;','&acute;','&micro;','&para;','&middot;','&cedil;','&sup1;','&ordm;','&raquo;','&frac14;','&frac12;','&frac34;','&iquest;','&Agrave;','&Aacute;','&Acirc;','&Atilde;','&Auml;','&Aring;','&AElig;','&Ccedil;','&Egrave;','&Eacute;','&Ecirc;','&Euml;','&Igrave;','&Iacute;','&Icirc;','&Iuml;','&ETH;','&Ntilde;','&Ograve;','&Oacute;','&Ocirc;','&Otilde;','&Ouml;','&times;','&Oslash;','&Ugrave;','&Uacute;','&Ucirc;','&Uuml;','&Yacute;','&THORN;','&szlig;','&agrave;','&aacute;','&acirc;','&atilde;','&auml;','&aring;','&aelig;','&ccedil;','&egrave;','&eacute;','&ecirc;','&euml;','&igrave;','&iacute;','&icirc;','&iuml;','&eth;','&ntilde;','&ograve;','&oacute;','&ocirc;','&otilde;','&ouml;','&divide;','&oslash;','&ugrave;','&uacute;','&ucirc;','&uuml;','&yacute;','&thorn;','&yuml;');
	$str = str_replace($html,$xml,$str);
	$str = str_ireplace($html,$xml,$str);
	return $str;
}  	
?>
