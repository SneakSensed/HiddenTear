<?php
	file_put_contents("log.txt", $_SERVER["REMOTE_ADDR"]. " : " . $_GET["info"]. "\r\n", FILE_APPEND | LOCK_EX);
	//http://host.net/write.php?info=info_to_be_logged
?>
