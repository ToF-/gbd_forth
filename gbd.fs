64    CONSTANT MAXDRIVER
65536 CONSTANT MAXSTOP
480   CONSTANT MAXTIME

CREATE DRIVERS MAXDRIVER CELLS ALLOT
CREATE STOPS   MAXSTOP   CELLS ALLOT

VARIABLE #DRIVERS
VARIABLE &DRIVER
VARIABLE #STOPS

\ increments a variable
: 1+! ( addr -- )
  1 SWAP +! ;

\ address of a driver's gossip
\ same as drivers address
: DRIVER>GOSSIP ( driver -- driver.gossip )
  ;

\ address of a driver's number of stops
: DRIVER>#STOPS ( driver -- driver.maxstop )
  CELL+ ;

\ address of a driver's arrays of stops
: DRIVER>ROUTE  ( driver -- driver.stops )
  CELL+ CELL+ ;

\ create a new driver in the dictionary
\ save its address for later update
\ set its fields to zero
: NEW-DRIVER ( -- )
  HERE &DRIVER !
  0 #STOPS !
  0 , 0 , ;

\ create a new stop for the current driver
\ being created, keep track of # of stops
: NEW-STOP ( stop -- )
 , #STOPS 1+! ;

\ return the next driver reference in
\ the array of drivers
: NEXT-DRIVER& ( -- driver& )
  #DRIVERS @ CELLS DRIVERS + ;

\ add the driver currently created to the
\ array of drivers, incrementing #drivers
: ADD-DRIVER
  &DRIVER @ DUP
  #STOPS @ SWAP DRIVER>#STOPS !
  NEXT-DRIVER& !
  #DRIVERS 1+! ;

\ return the driver # index
: NTH-DRIVER ( index -- addr )
  CELLS DRIVERS + @ ;

\ return the stop for a driver at a given minute
: DRIVER-STOP ( driver,minute -- stop )
  OVER DRIVER>#STOPS @ MOD CELLS
  SWAP DRIVER>ROUTE + @ ;


\ given a driver d, return gossip bit
: GOSSIP-BIT ( d -- 2^d )
  1 SWAP LSHIFT ;

\ initialize each driver with 
\ their bit of gossip
: INIT-DRIVERS
  #DRIVERS @ 0 DO
    I GOSSIP-BIT
    I NTH-DRIVER DRIVER>GOSSIP !
  LOOP ;

\ clear stops from the past minute meetings
: CLEAR-STOPS
  STOPS MAXSTOP CELLS ERASE ;

\ return the stop # index
: NTH-STOP ( index -- addr )
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
    I NTH-DRIVER OVER DRIVER-STOP NTH-STOP
    I GOSSIP-BIT SWAP ADD-GOSSIP!
  LOOP DROP ;

\ union of all the gossip bits
\ coming for the drivers in the set
: COLLECT-GOSSIP ( set -- gossip )
  0 SWAP
  #DRIVERS @ 0 DO
    I GOSSIP-BIT OVER AND IF
      I NTH-DRIVER DRIVER>GOSSIP @
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
      I NTH-DRIVER DRIVER>GOSSIP
      OVER SWAP !
    THEN
  LOOP DROP DROP ;

\ for each stop where there's at least 
\ 1 driver, collect the gossip at that
\ stop then update the drivers
: DRIVERS-UPDATE! ( minute -- )
  MAXSTOP 0 DO
    I NTH-STOP @ ?DUP IF
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
    I NTH-DRIVER DRIVER>GOSSIP @
    AND
  LOOP
  = ;

\ compute the first minute when all
\ drivers have shared their gossip
\ or 0 if never
: TIME-TO-COMPLETE ( -- n )
  INIT-DRIVERS
  -1 
  MAXTIME 0 DO
    I DRIVERS-MEET!
    I DRIVERS-UPDATE!
    COMPLETE? IF DROP I LEAVE THEN
  LOOP 1+ ;


