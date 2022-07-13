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
