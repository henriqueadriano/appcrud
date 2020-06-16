SELECT Date, Message 
FROM Log 
WHERE Date BETWEEN datetime('@DateStart 00:00:00') AND datetime('@DateEnd 23:59:59') 
	AND LEVEL = 'INFO' AND instr(Message,'Form1') > 0;