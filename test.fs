\  42 17 23 42
\  42 23 42 17
\  04 23 42 04 05

REQUIRE ffl/tst.fs
REQUIRE gbd.fs

T{ ." add-driver" CR
}T

T{ ." new-driver new-stop add-driver" CR
  #DRIVERS @ 0 ?S
  CREATE Albert [| 42 , 17 , 23 , 42 |]
  #DRIVERS @ 1 ?S
}T

T{ ." driver-stop" CR
  Albert 0 DRIVER-STOP 42 ?S
  Albert 1 DRIVER-STOP 17 ?S
  Albert 2 DRIVER-STOP 23 ?S
  Albert 3 DRIVER-STOP 42 ?S
  Albert 4 DRIVER-STOP 42 ?S
  Albert 5 DRIVER-STOP 17 ?S
}T

T{ ." #drivers" CR
  CREATE Barnabe [| 42 , 23 , 42 , 17 |]
  CREATE Clara   [| 04 , 23 , 42 , 04 , 05 |]
  #DRIVERS @ 3 ?S
}T

T{ ." driver driver>gossip driver>#stops" CR
  0 NTH-DRIVER Albert = ?TRUE
  1 NTH-DRIVER Barnabe = ?TRUE
  2 NTH-DRIVER Clara = ?TRUE
  INIT-DRIVERS
  Albert  DRIVER>GOSSIP @ 1 ?S
  Barnabe DRIVER>GOSSIP @ 2 ?S
  Clara   DRIVER>GOSSIP @ 4 ?S
  Albert  DRIVER>#STOPS @ 4 ?S
  Barnabe DRIVER>#STOPS @ 4 ?S
  Clara   DRIVER>#STOPS @ 5 ?S
}T

T{ ." drivers-meet!" CR
  0 DRIVERS-MEET!
  42 NTH-STOP @  0 GOSSIP-BIT 1 GOSSIP-BIT OR ?S
  04 NTH-STOP @  2 GOSSIP-BIT ?S
}T

T{ ." drivers-update!" CR
  0 DRIVERS-UPDATE!
  Albert DRIVER>GOSSIP @ 0 GOSSIP-BIT 1 GOSSIP-BIT OR ?S
  Barnabe DRIVER>GOSSIP @ 0 GOSSIP-BIT 1 GOSSIP-BIT OR ?S
  Clara  DRIVER>GOSSIP @ 2 GOSSIP-BIT ?S
}T

T{ ." time-to-complete" CR
  TIME-TO-COMPLETE 5 ?S
}T

BYE


