
VARIABLE #DRIVERS

: BIT>VALUE ( b -- n )
    1 SWAP LSHIFT ;

 8 BIT>VALUE CONSTANT MAXDRIVER
16 BIT>VALUE CONSTANT MAXSTOP
60 8 *       CONSTANT MAXTIME

CREATE STOPS   MAXSTOP   CELLS ALLOT 
CREATE DRIVERS MAXDRIVER CELLS ALLOT
CREATE ROUTES  MAXDRIVER MAXTIME * CELLS ALLOT
CREATE CYCLES  MAXDRIVER CELLS ALLOT

: FULL  ( -- n )
    #DRIVERS @ BIT>VALUE 1- ;

: GOSSIP ( driver -- gossip )
    CELLS DRIVERS + ;

: COMPLETE ( -- b )
    FULL
    #DRIVERS @ 0 DO I GOSSIP @ AND LOOP
    FULL = ;
        
: SET+ ( n,set -- set' )
    SWAP BIT>VALUE OR ;
    
: IN-SET? ( n,set -- b )
    SWAP BIT>VALUE AND ;

: .SET ( drivers -- )
    #DRIVERS @ 0 DO
        DUP I SWAP IN-SET? IF I . THEN
    LOOP DROP ;

: CLEAR-DRIVERS
    DRIVERS MAXDRIVER CELLS ERASE ;

: INIT-DRIVERS
    #DRIVERS @ 0 DO
        I BIT>VALUE 
        DRIVERS I CELLS + !
    LOOP ;

: CLEAR-STOPS
    STOPS MAXSTOP CELLS ERASE ;

: CLEAR-ROUTES
    ROUTES MAXDRIVER MAXTIME * ERASE ;

: STOP-ADDR ( stop -- addr )
    CELLS STOPS + ;

: DRIVER-STOP! ( driver,stop -- )
    STOP-ADDR DUP @
    ROT SWAP SET+
    SWAP ! ;

: DRIVER-ADDR ( driver -- addr )
    CELLS DRIVERS + ;

: DRIVER-GOSSIP@ ( driver -- set )
    DRIVER-ADDR @ ;

: DRIVER-GOSSIP! ( gossip,driver -- )
    DRIVER-ADDR ! ;

: COLLECT-STOP-GOSSIP ( stop -- gossip )
    STOP-ADDR @                \ drivers
    0 SWAP                     \ acc,drivers
    #DRIVERS @ 0 DO
        I OVER IN-SET? IF      \ acc,drivers
            I DRIVER-GOSSIP@   \ acc,drivers,gossip
            ROT OR SWAP        \ acc',drivers 
    THEN LOOP DROP ;

: UPDATE-DRIVER-GOSSIP ( gossip,driver -- )
    DRIVER-ADDR DUP @ 
    ROT OR SWAP ! ;

: UPDATE-DRIVERS ( gossip,drivers -- )
    #DRIVERS @ 0 DO                  \ gossip,drivers
        DUP I SWAP IN-SET? IF        \ gossip,drivers
            OVER I UPDATE-DRIVER-GOSSIP
        THEN
    LOOP DROP DROP ;
            
: UPDATE-ALL-DRIVERS
    6 0 DO
        I STOP-ADDR @
        I COLLECT-STOP-GOSSIP
        SWAP UPDATE-DRIVERS
    LOOP ;

: DRIVER-STOP ( minute,driver -- stop )
    DUP CELLS CYCLES + @ SWAP
    MAXTIME * CELLS ROUTES + -ROT
    MOD CELLS + @ ;
    
: STORE-DRIVER-STOP ( stop,minute,driver -- )
    MAXTIME * + CELLS ROUTES + ! ;

: DRIVERS-STOPS! ( minute - )
    #DRIVERS @ 0 DO
        DUP I DRIVER-STOP
        I SWAP DRIVER-STOP!
    LOOP DROP ;

: .DRIVERS 
    #DRIVERS @ 0 DO
        I . ." ["
        I CELLS DRIVERS + @ 
        DUP FULL = IF DROP ." X" ELSE .SET THEN ." ]  "
    LOOP ;

: .STOPS
    ." STOPS:"
    #DRIVERS @ 0 DO
        DUP I DRIVER-STOP .
    LOOP CR DROP ;

: .STOPS@
    MAXSTOP 0 DO
        I CELLS STOPS + @ DUP IF 
            I . ." (" .SET ." )  " 
        ELSE 
            DROP 
        THEN
    LOOP CR ;

: TRACK-COMPLETE-GOSSIP 
    INIT-DRIVERS
    .DRIVERS CR
    0 
    BEGIN
        DUP .
        DUP .STOPS
        DUP MAXTIME < COMPLETE 0= AND WHILE
        CLEAR-STOPS
        DUP DRIVERS-STOPS!
        .STOPS@
        UPDATE-ALL-DRIVERS
        .DRIVERS CR
        1+
    REPEAT
    COMPLETE IF
        . CR
    ELSE
        DROP ." never" CR
    THEN ;

: SKIP-NON-DIGIT ( -- n )
    BEGIN KEY DIGIT? 0= WHILE REPEAT ;

: GET-NUMBER ( -- n )
    0 SKIP-NON-DIGIT
    BEGIN
        SWAP 10 * +
        KEY DIGIT?
    0= UNTIL ;
    
: GET-STOPS ( driver -- )
    DUP CELLS CYCLES + @
    0 DO
        DUP GET-NUMBER I ROT 
        STORE-DRIVER-STOP
    LOOP DROP ;

: GET-ROUTE ( driver -- )
    GET-NUMBER
    OVER CELLS CYCLES + !
    GET-STOPS ;

: GET-ROUTES
    GET-NUMBER
    DUP #DRIVERS !
    0 DO
        I GET-ROUTE
    LOOP ;

GET-ROUTES
TRACK-COMPLETE-GOSSIP
BYE

