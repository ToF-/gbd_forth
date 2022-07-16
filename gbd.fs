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
: DRIVER>GOSSIP ( addr -- addr )
  ;

\ address of a driver's number of stops
: DRIVER>#STOPS ( addr -- addr' )
  CELL+ ;

\ address of a driver's arrays of stops
: DRIVER>ROUTE  ( addr -- addr' )
  CELL+ CELL+ ;

\ create a new driver in the dictionary
\ leave its address for later update
\ set its fields to zero
: [| ( -- addr )
  HERE 0 , 0 , ;

\ given initial address compute
\ route length for the current driver
\ being added
: ADDED-ROUTE-LENGTH ( addr -- n )
  HERE SWAP - CELL / 2 - ;

\ return the next driver address
\ to be assigned in the drivers arrary
: NEXT-DRIVER& ( -- addr )
  #DRIVERS @ CELLS DRIVERS + ;

\ add last stop to the current driver
\ set the total # of stops
\ add the driver currently created to the
\ array of drivers, incrementing count
: |] ( addr -- )
  , DUP ADDED-ROUTE-LENGTH 
  OVER DRIVER>#STOPS !
  NEXT-DRIVER& !
  #DRIVERS 1+! ;

\ return the driver # index
: NTH-DRIVER ( n -- addr )
  CELLS DRIVERS + @ ;

\ which stop # for a given driver and minute 
: DRIVER-STOP ( addr,n -- n )
  OVER DRIVER>#STOPS @ MOD CELLS
  SWAP DRIVER>ROUTE + @ ;


\ given a driver d, return a bitset
\ containing only a bit for d
: GOSSIP-BIT ( n -- bs )
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

\ address on stop#n
: NTH-STOP ( n -- addr )
  CELLS STOPS + ;

\ union of two gossip sets
: ADD-GOSSIP ( bs1,bs2 -- bs1|bs2 )
  OR ;

\ add (union) gossip at the given set address
: ADD-GOSSIP! ( bs,addr -- )
  DUP @ ROT ADD-GOSSIP SWAP ! ;

\ have the stops collect the drivers
\ that are stopping at them at a given minute
: DRIVERS-MEET! ( n -- )
  CLEAR-STOPS
  #DRIVERS @ 0 DO
    I NTH-DRIVER OVER DRIVER-STOP NTH-STOP
    I GOSSIP-BIT SWAP ADD-GOSSIP!
  LOOP DROP ;

\ union of all the gossip bits
\ coming for the drivers in the set
: COLLECT-GOSSIP ( bs -- bs' )
  0 SWAP
  #DRIVERS @ 0 DO
    I GOSSIP-BIT OVER AND IF
      I NTH-DRIVER DRIVER>GOSSIP @
      ROT ADD-GOSSIP SWAP
    THEN
  LOOP DROP ;
      
\ true if gossip bit n is in the bitset
: IN-GOSSIP? ( n,bs -- f )
  SWAP GOSSIP-BIT AND ;

\ store gossip bs1 into each driver
\ present in the set bs2
: UPDATE-GOSSIP! ( bs1,bs2 -- )
  #DRIVERS @ 0 DO
    OVER I SWAP IN-GOSSIP? IF
      I NTH-DRIVER DRIVER>GOSSIP
      OVER SWAP !
    THEN
  LOOP DROP DROP ;

\ for each stop where there's at least 
\ 1 driver, collect the gossip at that
\ stop then update the drivers
: DRIVERS-UPDATE! ( n -- )
  MAXSTOP 0 DO
    I NTH-STOP @ ?DUP IF
      DUP COLLECT-GOSSIP
      UPDATE-GOSSIP!
    THEN
  LOOP DROP ;

\ full bitset for size = n
: FULL-GOSSIP ( n -- bs )
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

