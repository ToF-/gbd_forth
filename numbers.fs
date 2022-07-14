: STR++ ( str -- str' )
  1- SWAP 1+ SWAP ;

: IS-SPACE? ( c -- b )
  DUP BL = SWAP 9 = OR ;

: SKIP-SPACES ( str -- str' )
  BEGIN 
    OVER C@ IS-SPACE? 
    OVER 0 > AND WHILE STR++ 
  REPEAT ;

: SKIP-NON-SPACES ( str -- str' )
  BEGIN
    OVER C@ IS-SPACE? 0=
    OVER 0 > AND WHILE STR++
  REPEAT ;

: S>NEXT-NUMBER? ( str -- str',d,f )
  SKIP-SPACES
  DUP IF
    2DUP SKIP-NON-SPACES    \ c-addr,n,c-addr',n'
    DUP -ROT 2>R -          \ c-addr,n''  [c-addr',n']
    S>NUMBER? 2R>           \ d,f,str'
    ROT >R 2SWAP R>         \ str',d,f
  ELSE
    0 S>D 0
  THEN ;

: S>NUMBERS! ( str,addr -- n )
  0 >R
  BEGIN      \ str,addr
    >R 
    S>NEXT-NUMBER? R> SWAP WHILE  \ str',d,addr
    DUP 2SWAP ROT 2!
    CELL+ CELL+
    R> 1+ >R
  REPEAT     \ str,addr
  DROP 2DROP 2DROP R> ;


