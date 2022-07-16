64    CONSTANT MAXDRIVER
65536 CONSTANT MAXSTOP
480   CONSTANT MAXTIME

CREATE DRIVERS MAXDRIVER CELLS ALLOT
CREATE STOPS   MAXSTOP   CELLS ALLOT

VARIABLE #DRIVERS
VARIABLE LAST-DRIVER
VARIABLE #STOPS

\ given a driver d, return gossip bit
: GOSSIP-BIT ( d -- 2^d )
  1 SWAP LSHIFT ;

\ address of a driver's gossip
: DRIVER>GOSSIP ( driver -- driver.gossip )
  ;

\ address of a driver's number of stops
: DRIVER>#STOPS ( driver -- driver.maxstop )
  CELL+ ;

\ address of a driver's arrays of stops
: DRIVER>ROUTE  ( driver -- driver.stops )
  CELL+ CELL+ ;

\ create a new driver in the dictionary
: NEW-DRIVER ( -- )
  HERE
  #DRIVERS @ GOSSIP-BIT , 0 , 
  LAST-DRIVER ! 
  0 #STOPS ! ;

\ create a new stop for the current driver
\ being created
: NEW-STOP ( stop -- )
 , 
 1 #STOPS +! ;

\ add the driver currently created to the
\ array of drivers, incrementing #drivers
: ADD-DRIVER
  LAST-DRIVER @
  #STOPS @ OVER DRIVER>#STOPS !
  #DRIVERS @ CELLS
  DRIVERS + !
  1 #DRIVERS +! ;

\ return the stop for a driver at a given minute
: DRIVER-STOP ( driver,minute -- stop )
  OVER DRIVER>#STOPS @ MOD CELLS
  SWAP DRIVER>ROUTE + @ ;

\ return the driver # index
: DRIVER ( index -- addr )
  CELLS DRIVERS + @ ;

\ clear stops from the past minute meetings
: CLEAR-STOPS
  STOPS MAXSTOP CELLS ERASE ;

\ return the stop # index
: STOP ( index -- addr )
  CELLS STOPS + ;

\ union of two gossip sets
: ADD-GOSSIP ( set,set' -- set'' )
  OR ;

\ add gossip at the given set address
: ADD-GOSSIP! ( n,addr -- addr|=n )
  DUP @ ROT ADD-GOSSIP SWAP ! ;

\ have the stops collect the drivers
\ that are stopping at them at a given minute
: DRIVERS-MEET! ( minute -- )
  CLEAR-STOPS
  #DRIVERS @ 0 DO
    I DRIVER OVER DRIVER-STOP STOP
    I GOSSIP-BIT SWAP ADD-GOSSIP!
  LOOP DROP ;

\ union of all the gossip bits
\ coming for the drivers in the set
: COLLECT-GOSSIP ( set -- gossip )
  0 SWAP
  #DRIVERS @ 0 DO
    I GOSSIP-BIT OVER AND IF
      I DRIVER DRIVER>GOSSIP @
      ROT ADD-GOSSIP SWAP
    THEN
  LOOP DROP ;
      
\ true if gossip bit i is in the set
: IN-GOSSIP? ( i,set -- f )
  SWAP GOSSIP-BIT AND ;

\ store gossip into each driver
\ present in the set
: UPDATE-GOSSIP! ( set,gossip -- )
  #DRIVERS @ 0 DO
    OVER I SWAP IN-GOSSIP? IF
      I DRIVER DRIVER>GOSSIP
      OVER SWAP !
    THEN
  LOOP DROP DROP ;

\ for each stop where there's at least 
\ 1 driver, collect the gossip at that
\ stop then update the drivers
: DRIVERS-UPDATE! ( minute -- )
  MAXSTOP 0 DO
    I STOP @ ?DUP IF
      DUP COLLECT-GOSSIP
      UPDATE-GOSSIP!
    THEN
  LOOP DROP ;

\ full bit set for size = n
: FULL-GOSSIP ( n -- set )
  GOSSIP-BIT 1- ;

\ true if all drivers have full gossip
: COMPLETE? ( -- f )
  #DRIVERS @  DUP         \ n,n
  FULL-GOSSIP DUP         \ n,f,f
  ROT 0 DO
    I DRIVER DRIVER>GOSSIP @
    AND
  LOOP
  = ;

\ initialize each driver with 
\ their bit of gossip
: INIT-DRIVERS
  #DRIVERS @ 0 DO
    I GOSSIP-BIT
    I DRIVER DRIVER>GOSSIP !
  LOOP ;

\ compute the first minute when all
\ drivers have shared their gossip
\ or 0 if never
: GDB ( -- n )
  INIT-DRIVERS
  -1 
  MAXTIME 0 DO
    I DRIVERS-MEET!
    I DRIVERS-UPDATE!
    COMPLETE? IF DROP I LEAVE THEN
  LOOP 1+ ;


