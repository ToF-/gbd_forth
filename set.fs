: BIT-VALUE ( n -- u )
  1 SWAP LSHIFT ;

: IN-SET? ( set,n -- b )
  BIT-VALUE AND ;

: IN-SET! ( n,set -- set' )
  SWAP BIT-VALUE OR ;

: FULL-SET ( size -- set )
  BIT-VALUE 1- ;

0 CONSTANT EMPTY-SET

: UNION-SET ( A,B -- A v B )
  OR ;

: SET-ITEMS ( set,max -- n1,n2,n3 )
  0 DO 
    DUP I IN-SET? IF 
      I SWAP 
    THEN 
  LOOP DROP ;
  
