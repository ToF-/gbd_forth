64    CONSTANT MAXDRIVER
65536 CONSTANT MAXSTOP
480   CONSTANT MAXTIME

CREATE DRIVERS MAXDRIVER CELLS ALLOT
CREATE STOPS   MAXSTOP   CELLS ALLOT

VARIABLE #DRIVERS
VARIABLE LAST-DRIVER
VARIABLE #STOPS

: GOSSIP-BIT ( d -- 2^d )
  1 SWAP LSHIFT ;

: DRIVER>GOSSIP ( driver -- driver.gossip )
  ;

: DRIVER>#STOPS ( driver -- driver.maxstop )
  CELL+ ;

: DRIVER>ROUTE  ( driver -- driver.stops )
  CELL+ CELL+ ;

: NEW-DRIVER ( -- )
  HERE
  #DRIVERS @ GOSSIP-BIT , 0 , 
  LAST-DRIVER ! 
  0 #STOPS ! ;

: NEXT-STOP ( driver -- driver.stops[driver.maxstop] )
  DUP DRIVER>#STOPS @ CELLS
  SWAP DRIVER>ROUTE + ;

: NEW-STOP ( stop -- )
 , 
 1 #STOPS +! ;

: ADD-DRIVER
  LAST-DRIVER @
  #STOPS @ OVER DRIVER>#STOPS !
  #DRIVERS @ CELLS
  DRIVERS + !
  1 #DRIVERS +! ;

: DRIVER-STOP ( driver,minute -- stop )
  OVER DRIVER>#STOPS @ MOD CELLS
  SWAP DRIVER>ROUTE + @ ;

: DRIVER ( index -- addr )
  CELLS DRIVERS + @ ;

: CLEAR-STOPS
  STOPS MAXSTOP CELLS ERASE ;

: STOP ( index -- addr )
  CELLS STOPS + ;

: OR! ( n,addr -- addr|=n )
  DUP @ ROT OR SWAP ! ;

: DRIVERS-MEET! ( minute -- )
  CLEAR-STOPS
  #DRIVERS @ 0 DO
    I DRIVER OVER DRIVER-STOP STOP
    I GOSSIP-BIT SWAP OR!
  LOOP DROP ;

: COLLECT-GOSSIP ( set -- gossip )
  0 SWAP
  #DRIVERS @ 0 DO
    I GOSSIP-BIT OVER AND IF
      I DRIVER DRIVER>GOSSIP @
      ROT OR SWAP
    THEN
  LOOP DROP ;
      
: IN-SET? ( i,set -- f )
  SWAP GOSSIP-BIT AND ;

: UPDATE-GOSSIP! ( set,gossip -- )
  #DRIVERS @ 0 DO
    OVER I SWAP IN-SET? IF
      I DRIVER DRIVER>GOSSIP
      OVER SWAP !
    THEN
  LOOP DROP DROP ;
  
: DRIVERS-UPDATE! ( minute -- )
  MAXSTOP 0 DO
    I STOP @ ?DUP IF
      DUP COLLECT-GOSSIP
      UPDATE-GOSSIP!
    THEN
  LOOP DROP ;

: FULL-GOSSIP ( n -- set )
  GOSSIP-BIT 1- ;

: COMPLETE ( -- f )
  #DRIVERS @  DUP         \ n,n
  FULL-GOSSIP DUP         \ n,f,f
  ROT 0 DO
    I DRIVER DRIVER>GOSSIP @
    AND
  LOOP
  = ;

: INIT-DRIVERS
  #DRIVERS @ 0 DO
    I GOSSIP-BIT
    I DRIVER DRIVER>GOSSIP !
  LOOP ;

: GDB ( -- n )
  INIT-DRIVERS
  -1 MAXTIME 0 DO
    I DRIVERS-MEET!
    I DRIVERS-UPDATE!
    COMPLETE IF
      DROP I LEAVE
    THEN
  LOOP 1+ ;


