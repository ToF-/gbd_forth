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

: ADDR++ ( addr,u -- addr+1,u-1 )
  1- SWAP 1+ SWAP ;

: MINUS? ( c -- b )
  [CHAR] - = ;

: >SNUMBER ( d,addr,u -- d',addr',u' )
  DUP 1 > IF
    OVER C@ MINUS? IF ADDR++ -1 ELSE 0 THEN
    >R >NUMBER 2SWAP R> IF DNEGATE THEN 
    2SWAP
  THEN ;
  
: SPACE? ( c -- b )
  DUP 32 = SWAP 9 = OR ;

: SKIP-SPACES ( addr,l -- addr',l' )
  BEGIN 
    OVER C@ SPACE? OVER 0 > AND WHILE
    ADDR++ 
  REPEAT ;

: NUMBERS ( addr,l,dest -- n )
  DUP 2SWAP
  BEGIN
    SKIP-SPACES
    DUP >R 0 0 2SWAP >SNUMBER DUP R> <> WHILE
      2ROT 2ROT D>S OVER !
      CELL+ 2SWAP
  REPEAT
  2DROP 2DROP SWAP - CELL / ;
