
 0x600055f LightingModel.LM_ITE_RGB.GetDeviceSupport recovered
000c ldarg.1 None
000d brfalse 1615
0012 ldarg.0 None
0013 ldfld 0x400091d LightingModel.LM_ITE_RGB.EcCtrl
0018 callvirt 0x6000646 MyECIO.MyEcCtrl.GetBiosProjctID
001d stloc.1 None
001e ldloca.s local(0x0001)
0020 constrained. 0x200006d Define.BIOS_PROJECT_ID
0026 callvirt 0xa000002 ToString
002b stloc.0 None
002c ldsfld 0x400280e 	..	
0031 ldc.i4 18
0036 ldelem.ref None
0037 ldloc.0 None
0038 ldstr '\x08\x14\r'
003d call 0x6001c6f 	.	.	
0042 callvirt 0x6001fd7 	..Invoke
0047 brtrue.s 105
0049 ldsfld 0x400280e 	..	
004e ldc.i4 18
0053 ldelem.ref None
0054 ldloc.0 None
0055 ldstr '\x08\x14\x0e'
005a call 0x6001c6f 	.	.	
005f callvirt 0x6001fd7 	..Invoke
0064 brfalse 840
0069 newobj 0xa000b22 .ctor
006e stloc.2 None
006f ldloc.2 None
0070 ldc.i4.0 None
0071 callvirt 0xa000b23 Add
0076 ldloc.2 None
0077 ldc.i4 162
007c callvirt 0xa000b23 Add
0081 ldloc.2 None
0082 ldc.i4.0 None
0083 callvirt 0xa000b23 Add
0088 ldloc.2 None
0089 ldc.i4.0 None
008a callvirt 0xa000b23 Add
008f ldloc.2 None
0090 ldc.i4.0 None
0091 callvirt 0xa000b23 Add
0096 ldloc.2 None
0097 ldc.i4.0 None
0098 callvirt 0xa000b23 Add
009d ldloc.2 None
009e ldc.i4.0 None
009f callvirt 0xa000b23 Add
00a4 ldloc.2 None
00a5 ldc.i4.0 None
00a6 callvirt 0xa000b23 Add
00ab ldloc.2 None
00ac ldc.i4.0 None
00ad callvirt 0xa000b23 Add
00b2 ldarg.0 None
00b3 ldfld 0x400091e LightingModel.LM_ITE_RGB.m_HIDManager
00b8 ldloc.2 None
00b9 callvirt 0xa000b24 ToArray
00be callvirt 0x60004ae UsbHidModel.HIDManager.WriteFeature
00c3 pop None
00c4 ldsfld 0x400280e 	..	
00c9 ldc.i4 6
00ce ldelem.ref None
00cf ldc.i4.1 None
00d0 callvirt 0x6001fbf 	.	.Invoke
00d5 ldc.i4.s 9
00d7 newarr 0x1000047 System.Byte
00dc stloc.3 None
00dd ldarg.0 None
00de ldfld 0x400091e LightingModel.LM_ITE_RGB.m_HIDManager
00e3 ldloc.3 None
00e4 callvirt 0x60004af UsbHidModel.HIDManager.GetFeature
00e9 pop None
00ea ldc.i4 255
00ef stloc.s local(0x0004)
00f1 ldloc.3 None
00f2 ldc.i4.2 None
00f3 ldelem.u1 None
00f4 stloc.s local(0x0004)
00f6 ldsfld 0x400280e 	..	
00fb ldc.i4 29
0100 ldelem.ref None
0101 ldstr '\x01\ufffb\ufff7`�\ufff7\ufff0QVUTSRK￨\x12\x13\x16F\ufff9\x1d\x13\x07AV?'
0106 call 0x6001c6f 	.	.	
010b ldarg.0 None
010c ldfld 0x400091d LightingModel.LM_ITE_RGB.EcCtrl
0111 callvirt 0x6000646 MyECIO.MyEcCtrl.GetBiosProjctID
0116 stloc.1 None
0117 ldloca.s local(0x0001)
0119 constrained. 0x200006d Define.BIOS_PROJECT_ID
011f callvirt 0xa000002 ToString
0124 callvirt 0x6001fed 	.	.Invoke
0129 call 0x60006e7 Utility.LogCtrl.Write
012e ldsfld 0x400280e 	..	
0133 ldc.i4 29
0138 ldelem.ref None
0139 ldstr '&2,I".+HA@?>=B�\ufff7\x04\ufff6\ufff1\ufffe\ufffa￬9￩￪￦￥\ufff1￥￦1\x1a/'
013e call 0x6001c6f 	.	.	
0143 ldsfld 0x400280e 	..	
0148 ldc.i4 55
014d ldelem.ref None
014e ldloca.s local(0x0004)
0150 callvirt 0x6002021 	.	.Invoke
0155 callvirt 0x6001fed 	.	.Invoke
015a call 0x60006e7 Utility.LogCtrl.Write
015f ldloc.s local(0x0004)
0161 switch [741, 642, 609, 708, 774, 510, 477, 411, 675, 543, 576, 444]
0196 br 807
019b ldarg.0 None
019c ldc.i4.1 None
019d stfld 0x4000955 LightingModel.LM_ITE_RGB.hingesupport
01a2 ldarg.0 None
01a3 ldc.i4.1 None
01a4 stfld 0x4000956 LightingModel.LM_ITE_RGB.basesupport
01a9 ldarg.0 None
01aa ldc.i4.1 None
01ab stfld 0x4000954 LightingModel.LM_ITE_RGB.logosupport
01b0 ldarg.0 None
01b1 ldc.i4.0 None
01b2 stfld 0x4000957 LightingModel.LM_ITE_RGB.newlogosupport
01b7 br 1326
01bc ldarg.0 None
01bd ldc.i4.1 None
01be stfld 0x4000955 LightingModel.LM_ITE_RGB.hingesupport
01c3 ldarg.0 None
01c4 ldc.i4.1 None
01c5 stfld 0x4000956 LightingModel.LM_ITE_RGB.basesupport
01ca ldarg.0 None
01cb ldc.i4.0 None
01cc stfld 0x4000954 LightingModel.LM_ITE_RGB.logosupport
01d1 ldarg.0 None
01d2 ldc.i4.1 None
01d3 stfld 0x4000957 LightingModel.LM_ITE_RGB.newlogosupport
01d8 br 1326
01dd ldarg.0 None
01de ldc.i4.1 None
01df stfld 0x4000955 LightingModel.LM_ITE_RGB.hingesupport
01e4 ldarg.0 None
01e5 ldc.i4.0 None
01e6 stfld 0x4000956 LightingModel.LM_ITE_RGB.basesupport
01eb ldarg.0 None
01ec ldc.i4.1 None
01ed stfld 0x4000954 LightingModel.LM_ITE_RGB.logosupport
01f2 ldarg.0 None
01f3 ldc.i4.0 None
01f4 stfld 0x4000957 LightingModel.LM_ITE_RGB.newlogosupport
01f9 br 1326
01fe ldarg.0 None
01ff ldc.i4.0 None
0200 stfld 0x4000955 LightingModel.LM_ITE_RGB.hingesupport
0205 ldarg.0 None
0206 ldc.i4.1 None
0207 stfld 0x4000956 LightingModel.LM_ITE_RGB.basesupport
020c ldarg.0 None
020d ldc.i4.0 None
020e stfld 0x4000954 LightingModel.LM_ITE_RGB.logosupport
0213 ldarg.0 None
0214 ldc.i4.1 None
0215 stfld 0x4000957 LightingModel.LM_ITE_RGB.newlogosupport
021a br 1326
021f ldarg.0 None
0220 ldc.i4.0 None
0221 stfld 0x4000955 LightingModel.LM_ITE_RGB.hingesupport
0226 ldarg.0 None
0227 ldc.i4.1 None
0228 stfld 0x4000956 LightingModel.LM_ITE_RGB.basesupport
022d ldarg.0 None
022e ldc.i4.0 None
022f stfld 0x4000954 LightingModel.LM_ITE_RGB.logosupport
0234 ldarg.0 None
0235 ldc.i4.1 None
0236 stfld 0x4000957 LightingModel.LM_ITE_RGB.newlogosupport
023b br 1326
0240 ldarg.0 None
0241 ldc.i4.1 None
0242 stfld 0x4000955 LightingModel.LM_ITE_RGB.hingesupport
0247 ldarg.0 None
0248 ldc.i4.0 None
0249 stfld 0x4000956 LightingModel.LM_ITE_RGB.basesupport
024e ldarg.0 None
024f ldc.i4.0 None
0250 stfld 0x4000954 LightingModel.LM_ITE_RGB.logosupport
0255 ldarg.0 None
0256 ldc.i4.1 None
0257 stfld 0x4000957 LightingModel.LM_ITE_RGB.newlogosupport
025c br 1326
0261 ldarg.0 None
0262 ldc.i4.1 None
0263 stfld 0x4000955 LightingModel.LM_ITE_RGB.hingesupport
0268 ldarg.0 None
0269 ldc.i4.0 None
026a stfld 0x4000956 LightingModel.LM_ITE_RGB.basesupport
026f ldarg.0 None
0270 ldc.i4.0 None
0271 stfld 0x4000954 LightingModel.LM_ITE_RGB.logosupport
0276 ldarg.0 None
0277 ldc.i4.0 None
0278 stfld 0x4000957 LightingModel.LM_ITE_RGB.newlogosupport
027d br 1326
0282 ldarg.0 None
0283 ldc.i4.0 None
0284 stfld 0x4000955 LightingModel.LM_ITE_RGB.hingesupport
0289 ldarg.0 None
028a ldc.i4.1 None
028b stfld 0x4000956 LightingModel.LM_ITE_RGB.basesupport
0290 ldarg.0 None
0291 ldc.i4.0 None
0292 stfld 0x4000954 LightingModel.LM_ITE_RGB.logosupport
0297 ldarg.0 None
0298 ldc.i4.0 None
0299 stfld 0x4000957 LightingModel.LM_ITE_RGB.newlogosupport
029e br 1326
02a3 ldarg.0 None
02a4 ldc.i4.0 None
02a5 stfld 0x4000955 LightingModel.LM_ITE_RGB.hingesupport
02aa ldarg.0 None
02ab ldc.i4.0 None
02ac stfld 0x4000956 LightingModel.LM_ITE_RGB.basesupport
02b1 ldarg.0 None
02b2 ldc.i4.0 None
02b3 stfld 0x4000954 LightingModel.LM_ITE_RGB.logosupport
02b8 ldarg.0 None
02b9 ldc.i4.1 None
02ba stfld 0x4000957 LightingModel.LM_ITE_RGB.newlogosupport
02bf br 1326
02c4 ldarg.0 None
02c5 ldc.i4.1 None
02c6 stfld 0x4000955 LightingModel.LM_ITE_RGB.hingesupport
02cb ldarg.0 None
02cc ldc.i4.1 None
02cd stfld 0x4000956 LightingModel.LM_ITE_RGB.basesupport
02d2 ldarg.0 None
02d3 ldc.i4.0 None
02d4 stfld 0x4000954 LightingModel.LM_ITE_RGB.logosupport
02d9 ldarg.0 None
02da ldc.i4.0 None
02db stfld 0x4000957 LightingModel.LM_ITE_RGB.newlogosupport
02e0 br 1326
02e5 ldarg.0 None
02e6 ldc.i4.1 None
02e7 stfld 0x4000955 LightingModel.LM_ITE_RGB.hingesupport
02ec ldarg.0 None
02ed ldc.i4.0 None
02ee stfld 0x4000956 LightingModel.LM_ITE_RGB.basesupport
02f3 ldarg.0 None
02f4 ldc.i4.0 None
02f5 stfld 0x4000954 LightingModel.LM_ITE_RGB.logosupport
02fa ldarg.0 None
02fb ldc.i4.0 None
02fc stfld 0x4000957 LightingModel.LM_ITE_RGB.newlogosupport
0301 br 1326
0306 ldarg.0 None
0307 ldc.i4.1 None
0308 stfld 0x4000955 LightingModel.LM_ITE_RGB.hingesupport
030d ldarg.0 None
030e ldc.i4.1 None
030f stfld 0x4000956 LightingModel.LM_ITE_RGB.basesupport
0314 ldarg.0 None
0315 ldc.i4.1 None
0316 stfld 0x4000954 LightingModel.LM_ITE_RGB.logosupport
031b ldarg.0 None
031c ldc.i4.0 None
031d stfld 0x4000957 LightingModel.LM_ITE_RGB.newlogosupport
0322 br 1326
0327 ldarg.0 None
0328 ldc.i4.0 None
0329 stfld 0x4000955 LightingModel.LM_ITE_RGB.hingesupport
032e ldarg.0 None
032f ldc.i4.0 None
0330 stfld 0x4000956 LightingModel.LM_ITE_RGB.basesupport
0335 ldarg.0 None
0336 ldc.i4.0 None
0337 stfld 0x4000954 LightingModel.LM_ITE_RGB.logosupport
033c ldarg.0 None
033d ldc.i4.0 None
033e stfld 0x4000957 LightingModel.LM_ITE_RGB.newlogosupport
0343 br 1326
0348 ldloc.0 None
0349 ldstr '\x08\x14\ufff7'
034e call 0x6001c6f 	.	.	
0353 call 0xa00000b op_Equality
0358 brtrue.s 890
035a ldsfld 0x400280e 	..	
035f ldc.i4 18
0364 ldelem.ref None
0365 ldloc.0 None
0366 ldstr '\x08\x14\ufff5'
036b call 0x6001c6f 	.	.	
0370 callvirt 0x6001fd7 	..Invoke
0375 brfalse 1326
037a newobj 0xa000b22 .ctor
037f stloc.s local(0x0005)
0381 ldloc.s local(0x0005)
0383 ldc.i4.0 None
0384 callvirt 0xa000b23 Add
0389 ldloc.s local(0x0005)
038b ldc.i4 162
0390 callvirt 0xa000b23 Add
0395 ldloc.s local(0x0005)
0397 ldc.i4.0 None
0398 callvirt 0xa000b23 Add
039d ldloc.s local(0x0005)
039f ldc.i4.0 None
03a0 callvirt 0xa000b23 Add
03a5 ldloc.s local(0x0005)
03a7 ldc.i4.0 None
03a8 callvirt 0xa000b23 Add
03ad ldloc.s local(0x0005)
03af ldc.i4.0 None
03b0 callvirt 0xa000b23 Add
03b5 ldloc.s local(0x0005)
03b7 ldc.i4.0 None
03b8 callvirt 0xa000b23 Add
03bd ldloc.s local(0x0005)
03bf ldc.i4.0 None
03c0 callvirt 0xa000b23 Add
03c5 ldloc.s local(0x0005)
03c7 ldc.i4.0 None
03c8 callvirt 0xa000b23 Add
03cd ldarg.0 None
03ce ldfld 0x400091e LightingModel.LM_ITE_RGB.m_HIDManager
03d3 ldloc.s local(0x0005)
03d5 callvirt 0xa000b24 ToArray
03da callvirt 0x60004ae UsbHidModel.HIDManager.WriteFeature
03df pop None
03e0 ldsfld 0x400280e 	..	
03e5 ldc.i4 6
03ea ldelem.ref None
03eb ldc.i4.1 None
03ec callvirt 0x6001fbf 	.	.Invoke
03f1 ldc.i4.s 9
03f3 newarr 0x1000047 System.Byte
03f8 stloc.s local(0x0006)
03fa ldarg.0 None
03fb ldfld 0x400091e LightingModel.LM_ITE_RGB.m_HIDManager
0400 ldloc.s local(0x0006)
0402 callvirt 0x60004af UsbHidModel.HIDManager.GetFeature
0407 pop None
0408 ldc.i4 255
040d stloc.s local(0x0007)
040f ldloc.s local(0x0006)
0411 ldc.i4.2 None
0412 ldelem.u1 None
0413 stloc.s local(0x0007)
0415 ldsfld 0x400280e 	..	
041a ldc.i4 29
041f ldelem.ref None
0420 ldstr '\x01\ufffb\x0e`�\ufff7\x08QVUTSRK￨\x12\x13\x16F\ufff9\x1d\x13\x07AV?'
0425 call 0x6001c6f 	.	.	
042a ldarg.0 None
042b ldfld 0x400091d LightingModel.LM_ITE_RGB.EcCtrl
0430 callvirt 0x6000646 MyECIO.MyEcCtrl.GetBiosProjctID
0435 stloc.1 None
0436 ldloca.s local(0x0001)
0438 constrained. 0x200006d Define.BIOS_PROJECT_ID
043e callvirt 0xa000002 ToString
0443 callvirt 0x6001fed 	.	.Invoke
0448 call 0x60006e7 Utility.LogCtrl.Write
044d ldsfld 0x400280e 	..	
0452 ldc.i4 29
0457 ldelem.ref None
0458 ldstr '&2\x15I".\x13HA@?>=B�\ufff7\x04\ufff6\ufff1\ufffe\ufffa￬9￩￪￦￥\ufff1￥￦1\x1a/'
045d call 0x6001c6f 	.	.	
0462 ldsfld 0x400280e 	..	
0467 ldc.i4 55
046c ldelem.ref None
046d ldloca.s local(0x0007)
046f callvirt 0x6002021 	.	.Invoke
0474 callvirt 0x6001fed 	.	.Invoke
0479 call 0x60006e7 Utility.LogCtrl.Write
047e ldloc.s local(0x0007)
0480 switch [1190, 1213, 1305, 1305, 1236, 1259, 1282]
04a1 br 1305
04a6 ldarg.0 None
04a7 ldc.i4.1 None
04a8 stfld 0x4000955 LightingModel.LM_ITE_RGB.hingesupport
04ad ldarg.0 None
04ae ldc.i4.0 None
04af stfld 0x4000956 LightingModel.LM_ITE_RGB.basesupport
04b4 ldarg.0 None
04b5 ldc.i4.0 None
04b6 stfld 0x4000954 LightingModel.LM_ITE_RGB.logosupport
04bb br.s 1326
04bd ldarg.0 None
04be ldc.i4.1 None
04bf stfld 0x4000955 LightingModel.LM_ITE_RGB.hingesupport
04c4 ldarg.0 None
04c5 ldc.i4.0 None
04c6 stfld 0x4000956 LightingModel.LM_ITE_RGB.basesupport
04cb ldarg.0 None
04cc ldc.i4.1 None
04cd stfld 0x4000954 LightingModel.LM_ITE_RGB.logosupport
04d2 br.s 1326
04d4 ldarg.0 None
04d5 ldc.i4.1 None
04d6 stfld 0x4000955 LightingModel.LM_ITE_RGB.hingesupport
04db ldarg.0 None
04dc ldc.i4.1 None
04dd stfld 0x4000956 LightingModel.LM_ITE_RGB.basesupport
04e2 ldarg.0 None
04e3 ldc.i4.1 None
04e4 stfld 0x4000954 LightingModel.LM_ITE_RGB.logosupport
04e9 br.s 1326
04eb ldarg.0 None
04ec ldc.i4.1 None
04ed stfld 0x4000955 LightingModel.LM_ITE_RGB.hingesupport
04f2 ldarg.0 None
04f3 ldc.i4.0 None
04f4 stfld 0x4000956 LightingModel.LM_ITE_RGB.basesupport
04f9 ldarg.0 None
04fa ldc.i4.0 None
04fb stfld 0x4000954 LightingModel.LM_ITE_RGB.logosupport
0500 br.s 1326
0502 ldarg.0 None
0503 ldc.i4.1 None
0504 stfld 0x4000955 LightingModel.LM_ITE_RGB.hingesupport
0509 ldarg.0 None
050a ldc.i4.1 None
050b stfld 0x4000956 LightingModel.LM_ITE_RGB.basesupport
0510 ldarg.0 None
0511 ldc.i4.0 None
0512 stfld 0x4000954 LightingModel.LM_ITE_RGB.logosupport
0517 br.s 1326
0519 ldarg.0 None
051a ldc.i4.0 None
051b stfld 0x4000955 LightingModel.LM_ITE_RGB.hingesupport
0520 ldarg.0 None
0521 ldc.i4.0 None
0522 stfld 0x4000956 LightingModel.LM_ITE_RGB.basesupport
0527 ldarg.0 None
0528 ldc.i4.0 None
0529 stfld 0x4000954 LightingModel.LM_ITE_RGB.logosupport
052e nop None
052f ldc.i4 -2147483646
0534 ldstr '\x10\ufffe\ufff7\ufffe\x0c\ufff2\x17\x1a\x1d\x15\r\ufff0\r\x11\x1a\n\x1aY\ufffe\ufff7￣￥￭\x02\x1d\x01\x05\x02\x0e\ufffb'
0539 call 0x6001c6f 	.	.	
053e ldstr '\r3\x17%&\x0e.%,'
0543 call 0x6001c6f 	.	.	
0548 ldarg.0 None
0549 ldfld 0x4000954 LightingModel.LM_ITE_RGB.logosupport
054e box 0x100001b System.Boolean
0553 ldc.i4.4 None
0554 call 0x60006ff Utility.RegistryCtrl.RegistrySoftwareKeyWrite
0559 ldc.i4 -2147483646
055e ldstr '\x10\ufffe\ufff7\ufffe\x0c\ufff2\x17\x1a\x1d\x15\r\ufff0\r\x11\x1a\n\x1aY\ufffe\ufff7￣￥￭\x02\x1d\x01\x05\x02\x0e\ufffb'
0563 call 0x6001c6f 	.	.	
0568 ldstr '\r3\x17%&\x04 \x11"'
056d call 0x6001c6f 	.	.	
0572 ldarg.0 None
0573 ldfld 0x4000956 LightingModel.LM_ITE_RGB.basesupport
0578 box 0x100001b System.Boolean
057d ldc.i4.4 None
057e call 0x60006ff Utility.RegistryCtrl.RegistrySoftwareKeyWrite
0583 ldc.i4 -2147483646
0588 ldstr '\x10\ufffe\ufff7\ufffe\x0c\ufff2\x17\x1a\x1d\x15\r\ufff0\r\x11\x1a\n\x1aY\ufffe\ufff7￣￥￭\x02\x1d\x01\x05\x02\x0e\ufffb'
058d call 0x6001c6f 	.	.	
0592 ldstr '\n2\x14$)\x0b++#$'
0597 call 0x6001c6f 	.	.	
059c ldarg.0 None
059d ldfld 0x4000955 LightingModel.LM_ITE_RGB.hingesupport
05a2 box 0x100001b System.Boolean
05a7 ldc.i4.4 None
05a8 call 0x60006ff Utility.RegistryCtrl.RegistrySoftwareKeyWrite
05ad ldc.i4 -2147483646
05b2 ldstr '\x10\ufffe\ufff7\ufffe\x0c\ufff2\x17\x1a\x1d\x15\r\ufff0\r\x11\x1a\n\x1aY\ufffe\ufff7￣￥￭\x02\x1d\x01\x05\x02\x0e\ufffb'
05b7 call 0x6001c6f 	.	.	
05bc ldstr '\x03\x19-\x1b \x16\x1e+\x15\x11\x18\x0f"\'#"\n\x1e#'
05c1 call 0x6001c6f 	.	.	
05c6 ldarg.0 None
05c7 ldfld 0x4000957 LightingModel.LM_ITE_RGB.newlogosupport
05cc box 0x100001b System.Boolean
05d1 ldc.i4.4 None
05d2 call 0x60006ff Utility.RegistryCtrl.RegistrySoftwareKeyWrite
05d7 ldc.i4.0 None
05d8 stloc.s local(0x0008)
05da ldarg.0 None
05db ldfld 0x400091d LightingModel.LM_ITE_RGB.EcCtrl
05e0 ldsfld 0x400280e 	..	
05e5 ldc.i4 128
05ea ldelem.ref None
05eb ldsfld 0x400280e 	..	
05f0 ldc.i4 121
05f5 ldelem.ref None
05f6 ldarg.0 None
05f7 callvirt 0x60020a5 	.

.Invoke
05fc callvirt 0x60020b3 	..Invoke
0601 ldc.i4 1963
0606 ldloca.s local(0x0008)
0608 callvirt 0x6000621 MyECIO.MyEcCtrl.Read
060d ldloc.s local(0x0008)
060f ldc.i4.1 None
0610 and None
0611 conv.u1 None
0612 stloc.s local(0x0008)
0614 ldloc.s local(0x0008)
0616 ldc.i4.1 None
0617 bne.un.s 1568
0619 ldarg.0 None
061a ldc.i4.1 None
061b stfld 0x4000958 LightingModel.LM_ITE_RGB.MBlogoSupport
0620 ldc.i4 -2147483646
0625 ldstr '\x10\ufffe\ufff7\ufffe\x0c\ufff2\x17\x1a\x1d\x15\r\ufff0\r\x11\x1a\n\x1aY\ufffe\ufff7￣￥￭\x02\x1d\x01\x05\x02\x0e\ufffb'
062a call 0x6001c6f 	.	.	
062f ldstr '\x0e\x0c--$+\ufff6\x0f\x13\x12&\x12\x0b'
0634 call 0x6001c6f 	.	.	
0639 ldarg.0 None
063a ldfld 0x4000958 LightingModel.LM_ITE_RGB.MBlogoSupport
063f box 0x100001b System.Boolean
0644 ldc.i4.4 None
0645 call 0x60006ff Utility.RegistryCtrl.RegistrySoftwareKeyWrite
064a leave.s 1615
064c pop None
064d leave.s 1615
064f nop None
0650 ldc.i4 -2147483646
0655 ldstr '\x10\ufffe\ufff7\ufffe\x0c\ufff2\x17\x1a\x1d\x15\r\ufff0\r\x11\x1a\n\x1aY\ufffe\ufff7￣￥￭\x02\x1d\x01\x05\x02\x0e\ufffb'
065a call 0x6001c6f 	.	.	
065f ldstr '\r3\x17%&\x0e.%,'
0664 call 0x6001c6f 	.	.	
0669 ldc.i4.0 None
066a box 0x1000012 System.Int32
066f call 0x60006fc Utility.RegistryCtrl.RegistrySoftwareKeyRead
0674 unbox.any 0x1000012 System.Int32
0679 stloc.s local(0x0009)
067b ldc.i4 -2147483646
0680 ldstr '\x10\ufffe\ufff7\ufffe\x0c\ufff2\x17\x1a\x1d\x15\r\ufff0\r\x11\x1a\n\x1aY\ufffe\ufff7￣￥￭\x02\x1d\x01\x05\x02\x0e\ufffb'
0685 call 0x6001c6f 	.	.	
068a ldstr '\r3\x17%&\x04 \x11"'
068f call 0x6001c6f 	.	.	
0694 ldc.i4.0 None
0695 box 0x1000012 System.Int32
069a call 0x60006fc Utility.RegistryCtrl.RegistrySoftwareKeyRead
069f unbox.any 0x1000012 System.Int32
06a4 stloc.s local(0x000A)
06a6 ldc.i4 -2147483646
06ab ldstr '\x10\ufffe\ufff7\ufffe\x0c\ufff2\x17\x1a\x1d\x15\r\ufff0\r\x11\x1a\n\x1aY\ufffe\ufff7￣￥￭\x02\x1d\x01\x05\x02\x0e\ufffb'
06b0 call 0x6001c6f 	.	.	
06b5 ldstr '\n2\x14$)\x0b++#$'
06ba call 0x6001c6f 	.	.	
06bf ldc.i4.0 None
06c0 box 0x1000012 System.Int32
06c5 call 0x60006fc Utility.RegistryCtrl.RegistrySoftwareKeyRead
06ca unbox.any 0x1000012 System.Int32
06cf stloc.s local(0x000B)
06d1 ldc.i4 -2147483646
06d6 ldstr '\x10\ufffe\ufff7\ufffe\x0c\ufff2\x17\x1a\x1d\x15\r\ufff0\r\x11\x1a\n\x1aY\ufffe\ufff7￣￥￭\x02\x1d\x01\x05\x02\x0e\ufffb'
06db call 0x6001c6f 	.	.	
06e0 ldstr '\x03\x19-\x1b \x16\x1e+\x15\x11\x18\x0f"\'#"\n\x1e#'
06e5 call 0x6001c6f 	.	.	
06ea ldc.i4.0 None
06eb box 0x1000012 System.Int32
06f0 call 0x60006fc Utility.RegistryCtrl.RegistrySoftwareKeyRead
06f5 unbox.any 0x1000012 System.Int32
06fa stloc.s local(0x000C)
06fc ldloc.s local(0x0009)
06fe ldc.i4.1 None
06ff bne.un.s 1817
0701 ldarg.0 None
0702 ldsfld 0x400280e 	..	
0707 ldc.i4 175
070c ldelem.ref None
070d ldloc.s local(0x0009)
070f callvirt 0x6002111 	.
.Invoke
0714 stfld 0x4000954 LightingModel.LM_ITE_RGB.logosupport
0719 ldloc.s local(0x000B)
071b ldc.i4.1 None
071c bne.un.s 1846
071e ldarg.0 None
071f ldsfld 0x400280e 	..	
0724 ldc.i4 175
0729 ldelem.ref None
072a ldloc.s local(0x000B)
072c callvirt 0x6002111 	.
.Invoke
0731 stfld 0x4000955 LightingModel.LM_ITE_RGB.hingesupport
0736 ldloc.s local(0x000A)
0738 ldc.i4.1 None
0739 bne.un.s 1875
073b ldarg.0 None
073c ldsfld 0x400280e 	..	
0741 ldc.i4 175
0746 ldelem.ref None
0747 ldloc.s local(0x000A)
0749 callvirt 0x6002111 	.
.Invoke
074e stfld 0x4000956 LightingModel.LM_ITE_RGB.basesupport
0753 ldloc.s local(0x000C)
0755 ldc.i4.1 None
0756 bne.un.s 1904
0758 ldarg.0 None
0759 ldsfld 0x400280e 	..	
075e ldc.i4 175
0763 ldelem.ref None
0764 ldloc.s local(0x000C)
0766 callvirt 0x6002111 	.
.Invoke
076b stfld 0x4000957 LightingModel.LM_ITE_RGB.newlogosupport
0770 leave.s 1959
0772 stloc.s local(0x000D)
0774 ldsfld 0x400280e 	..	
0779 ldc.i4 29
077e ldelem.ref None
077f ldstr ')\x0f\ufffb\t\x02F\t\x0b\x00\n￭\x02\ufffe\ufff0)<\ufff9\ufff5\ufff8￤￢￨\ufff0￥3\x04￬￭\ufff6\uffdf\uffd9\uffe7\uffdd*\x04￠￨￡ￕ\uffd0￪\uffe7\uffe7\x1a\x1f'
0784 call 0x6001c6f 	.	.	
0789 ldsfld 0x400280e 	..	
078e ldc.i4 1
0793 ldelem.ref None
0794 ldloc.s local(0x000D)
0796 callvirt 0x6001fb5 	.
.Invoke
079b callvirt 0x6001fed 	.	.Invoke
07a0 call 0x60006e7 Utility.LogCtrl.Write
07a5 leave.s 1959
07a7 ldstr '=?J\x149;D\x1d\x12\x11\x10\x0f\x0e\x17JNOK^OQ]\x0e\\Y[Z<VS\x06\x05<<4<=NKML.F\ufff9\ufffe\ufff7=\x059\uffe7&$+"=:<;\x1d74\uffe7￬￥+\ufff4\'ￕ\x1e -\x1a+(*)\x0b%"ￕￚￓ\x19\uffdf\x15ￏ'
07ac call 0x6001c6f 	.	.	
07b1 ldarg.0 None
07b2 ldfld 0x4000955 LightingModel.LM_ITE_RGB.hingesupport
07b7 box 0x100001b System.Boolean
07bc ldarg.0 None
07bd ldfld 0x4000954 LightingModel.LM_ITE_RGB.logosupport
07c2 box 0x100001b System.Boolean
07c7 ldarg.0 None
07c8 ldfld 0x4000956 LightingModel.LM_ITE_RGB.basesupport
07cd box 0x100001b System.Boolean
07d2 call 0xa000019 Format
07d7 call 0x60006e7 Utility.LogCtrl.Write
07dc ret None

 0x60017c0 MyControlCenter.MyRgbKeyboard.HIDKeyboard.RunEffctEC recovered
000c ldarg.0 None
000d ldfld 0x4002001 MyControlCenter.MyRgbKeyboard.HIDKeyboard.mbLogo
0012 brtrue.s 25
0014 leave 2630
0019 ldc.i4.0 None
001a stloc.0 None
001b ldarg.0 None
001c ldflda 0x4001fed MyControlCenter.MyRgbKeyboard.HIDKeyboard._PowerStatus
0021 ldc.i4.1 None
0022 box 0x20000cb LightingModel.RGBKB_PowerStatus
0027 constrained. 0x20000cb LightingModel.RGBKB_PowerStatus
002d callvirt 0xa000005 Equals
0032 brfalse 2392
0037 ldsfld 0x400280e 	..	
003c ldc.i4 29
0041 ldelem.ref None
0042 ldstr '\x0c&\x1e\ufff4\x14\x13\x17\x1f\uffef\ufff4XXRM\ufffe\x18\x10I￥\x05\x04\x02\x07\x0fB\n\x06?'
0047 call 0x6001c6f 	.	.	
004c ldsfld 0x400280e 	..	
0051 ldc.i4 119
0056 ldelem.ref None
0057 ldarg.0 None
0058 ldflda 0x4001ff1 MyControlCenter.MyRgbKeyboard.HIDKeyboard._EffectData
005d ldflda 0x4000915 LightingModel.SAVE_LIGHTING_EFFECT_DATA.save_effect
0062 callvirt 0x60020a1 	.
.Invoke
0067 callvirt 0x6001fed 	.	.Invoke
006c call 0x60006e7 Utility.LogCtrl.Write
0071 ldc.i4.0 None
0072 stloc.1 None
0073 ldc.i4.s 64
0075 stloc.2 None
0076 ldsfld 0x400280e 	..	
007b ldc.i4 161
0080 ldelem.ref None
0081 ldarg.0 None
0082 ldflda 0x4001ff1 MyControlCenter.MyRgbKeyboard.HIDKeyboard._EffectData
0087 ldfld 0x4000916 LightingModel.SAVE_LIGHTING_EFFECT_DATA.save_light
008c callvirt 0x60020f5 	.	.Invoke
0091 stloc.s local(0x0004)
0093 ldloc.s local(0x0004)
0095 ldc.i4.s 25
0097 bgt.un.s 165
0099 ldloc.s local(0x0004)
009b brfalse.s 185
009d ldloc.s local(0x0004)
009f ldc.i4.s 25
00a1 beq.s 189
00a3 br.s 210
00a5 ldloc.s local(0x0004)
00a7 ldc.i4.s 50
00a9 beq.s 194
00ab ldloc.s local(0x0004)
00ad ldc.i4.s 75
00af beq.s 199
00b1 ldloc.s local(0x0004)
00b3 ldc.i4.s 100
00b5 beq.s 204
00b7 br.s 210
00b9 ldc.i4.0 None
00ba stloc.2 None
00bb br.s 210
00bd ldc.i4.s 32
00bf stloc.2 None
00c0 br.s 210
00c2 ldc.i4.s 64
00c4 stloc.2 None
00c5 br.s 210
00c7 ldc.i4.s 96
00c9 stloc.2 None
00ca br.s 210
00cc ldc.i4 128
00d1 stloc.2 None
00d2 ldarg.0 None
00d3 ldflda 0x4001ff1 MyControlCenter.MyRgbKeyboard.HIDKeyboard._EffectData
00d8 ldfld 0x4000915 LightingModel.SAVE_LIGHTING_EFFECT_DATA.save_effect
00dd ldc.i4.1 None
00de bne.un 1032
00e3 ldc.i4.0 None
00e4 stloc.s local(0x0005)
00e6 call 0x600061b MyECIO.MyEcCtrl.get_Instance
00eb ldsfld 0x400280e 	..	
00f0 ldc.i4 128
00f5 ldelem.ref None
00f6 ldsfld 0x400280e 	..	
00fb ldc.i4 121
0100 ldelem.ref None
0101 ldarg.0 None
0102 callvirt 0x60020a5 	.

.Invoke
0107 callvirt 0x60020b3 	..Invoke
010c ldc.i4 1895
0111 ldloca.s local(0x0005)
0113 callvirt 0x6000621 MyECIO.MyEcCtrl.Read
0118 ldsfld 0x400280e 	..	
011d ldc.i4 29
0122 ldelem.ref None
0123 ldstr 'q(ppn\\\x08\x11\x13\x19\x13\x19U\x02\x16\x11\x15VO'
0128 call 0x6001c6f 	.	.	
012d ldsfld 0x400280e 	..	
0132 ldc.i4 119
0137 ldelem.ref None
0138 ldloca.s local(0x0005)
013a callvirt 0x60020a1 	.
.Invoke
013f callvirt 0x6001fed 	.	.Invoke
0144 ldstr '\ufff6\x1c0\n&%!\x15\x05\ufffe'
0149 call 0x6001c6f 	.	.	
014e ldstr '73LF]g_FbRSLB0/8A(@;+^\'JHMRFB 9AF6J+\x1bN\x17:8=B62\x10)16&:\x1b\x0b>\x16"$\x0c\x1d8 $\x1d-\x16\r\x00\x00\ufff2\x00\x11,\x14\x18\x11!\n\x01\ufff4\ufff4￦\ufff4\x05 \x08\x0c\x05\x15\ufffeￇ\x03\x12'
0153 call 0x6001c6f 	.	.	
0158 ldc.i4 1847
015d call 0x60006ee Utility.LogCtrl.TraceMessage
0162 ldloc.s local(0x0005)
0164 ldc.i4.s 127
0166 and None
0167 conv.u1 None
0168 stloc.s local(0x0005)
016a call 0x600061b MyECIO.MyEcCtrl.get_Instance
016f ldsfld 0x400280e 	..	
0174 ldc.i4 128
0179 ldelem.ref None
017a ldsfld 0x400280e 	..	
017f ldc.i4 121
0184 ldelem.ref None
0185 ldarg.0 None
0186 callvirt 0x60020a5 	.

.Invoke
018b callvirt 0x60020b3 	..Invoke
0190 ldc.i4 1895
0195 ldloc.s local(0x0005)
0197 callvirt 0x6000622 MyECIO.MyEcCtrl.Write
019c ldsfld 0x400280e 	..	
01a1 ldc.i4 29
01a6 ldelem.ref None
01a7 ldstr 'r)omm]\x0f\x14\x10\x18\x0c\x14V\x04&\x0c\x1e\x0eZO'
01ac call 0x6001c6f 	.	.	
01b1 ldsfld 0x400280e 	..	
01b6 ldc.i4 119
01bb ldelem.ref None
01bc ldloca.s local(0x0005)
01be callvirt 0x60020a1 	.
.Invoke
01c3 callvirt 0x6001fed 	.	.Invoke
01c8 ldstr '\ufff6\x1c0\n&%!\x15\x05\ufffe'
01cd call 0x6001c6f 	.	.	
01d2 ldstr '73LF]g_FbRSLB0/8A(@;+^\'JHMRFB 9AF6J+\x1bN\x17:8=B62\x10)16&:\x1b\x0b>\x16"$\x0c\x1d8 $\x1d-\x16\r\x00\x00\ufff2\x00\x11,\x14\x18\x11!\n\x01\ufff4\ufff4￦\ufff4\x05 \x08\x0c\x05\x15\ufffeￇ\x03\x12'
01d7 call 0x6001c6f 	.	.	
01dc ldc.i4 1850
01e1 call 0x60006ee Utility.LogCtrl.TraceMessage
01e6 ldc.i4.0 None
01e7 stloc.s local(0x0005)
01e9 call 0x600061b MyECIO.MyEcCtrl.get_Instance
01ee ldsfld 0x400280e 	..	
01f3 ldc.i4 128
01f8 ldelem.ref None
01f9 ldsfld 0x400280e 	..	
01fe ldc.i4 121
0203 ldelem.ref None
0204 ldarg.0 None
0205 callvirt 0x60020a5 	.

.Invoke
020a callvirt 0x60020b3 	..Invoke
020f ldc.i4 1989
0214 ldloca.s local(0x0005)
0216 callvirt 0x6000621 MyECIO.MyEcCtrl.Read
021b ldloc.s local(0x0005)
021d ldc.i4 248
0222 and None
0223 conv.u1 None
0224 stloc.s local(0x0005)
0226 call 0x600061b MyECIO.MyEcCtrl.get_Instance
022b ldsfld 0x400280e 	..	
0230 ldc.i4 128
0235 ldelem.ref None
0236 ldsfld 0x400280e 	..	
023b ldc.i4 121
0240 ldelem.ref None
0241 ldarg.0 None
0242 callvirt 0x60020a5 	.

.Invoke
0247 callvirt 0x60020b3 	..Invoke
024c ldc.i4 1989
0251 ldloc.s local(0x0005)
0253 callvirt 0x6000622 MyECIO.MyEcCtrl.Write
0258 ldarg.0 None
0259 ldflda 0x4001ff1 MyControlCenter.MyRgbKeyboard.HIDKeyboard._EffectData
025e ldflda 0x4000919 LightingModel.SAVE_LIGHTING_EFFECT_DATA.save_layout_color
0263 ldfld 0x400087b LightingModel.RGBKB_Color.ColorBuffer
0268 ldc.i4.0 None
0269 ldelema 0x20000d3 LightingModel.RGB_S
026e ldfld 0x4000876 LightingModel.RGB_S.R
0273 ldc.r8 5.1
027c call 0x60017c2 MyControlCenter.MyRgbKeyboard.HIDKeyboard.DivideValue
0281 stloc.s local(0x0006)
0283 ldarg.0 None
0284 ldflda 0x4001ff1 MyControlCenter.MyRgbKeyboard.HIDKeyboard._EffectData
0289 ldflda 0x4000919 LightingModel.SAVE_LIGHTING_EFFECT_DATA.save_layout_color
028e ldfld 0x400087b LightingModel.RGBKB_Color.ColorBuffer
0293 ldc.i4.0 None
0294 ldelema 0x20000d3 LightingModel.RGB_S
0299 ldfld 0x4000877 LightingModel.RGB_S.G
029e ldc.r8 5.1
02a7 call 0x60017c2 MyControlCenter.MyRgbKeyboard.HIDKeyboard.DivideValue
02ac stloc.s local(0x0007)
02ae ldarg.0 None
02af ldflda 0x4001ff1 MyControlCenter.MyRgbKeyboard.HIDKeyboard._EffectData
02b4 ldflda 0x4000919 LightingModel.SAVE_LIGHTING_EFFECT_DATA.save_layout_color
02b9 ldfld 0x400087b LightingModel.RGBKB_Color.ColorBuffer
02be ldc.i4.0 None
02bf ldelema 0x20000d3 LightingModel.RGB_S
02c4 ldfld 0x4000878 LightingModel.RGB_S.B
02c9 ldc.r8 5.1
02d2 call 0x60017c2 MyControlCenter.MyRgbKeyboard.HIDKeyboard.DivideValue
02d7 stloc.s local(0x0008)
02d9 ldloc.s local(0x0006)
02db brtrue.s 748
02dd ldloc.s local(0x0007)
02df brtrue.s 748
02e1 ldloc.s local(0x0008)
02e3 brtrue.s 748
02e5 ldc.i4.1 None
02e6 stloc.0 None
02e7 br 2116
02ec call 0x600061b MyECIO.MyEcCtrl.get_Instance
02f1 ldsfld 0x400280e 	..	
02f6 ldc.i4 128
02fb ldelem.ref None
02fc ldsfld 0x400280e 	..	
0301 ldc.i4 121
0306 ldelem.ref None
0307 ldarg.0 None
0308 callvirt 0x60020a5 	.

.Invoke
030d callvirt 0x60020b3 	..Invoke
0312 ldc.i4 1897
0317 ldloc.s local(0x0006)
0319 callvirt 0x6000622 MyECIO.MyEcCtrl.Write
031e call 0x600061b MyECIO.MyEcCtrl.get_Instance
0323 ldsfld 0x400280e 	..	
0328 ldc.i4 128
032d ldelem.ref None
032e ldsfld 0x400280e 	..	
0333 ldc.i4 121
0338 ldelem.ref None
0339 ldarg.0 None
033a callvirt 0x60020a5 	.

.Invoke
033f callvirt 0x60020b3 	..Invoke
0344 ldc.i4 1898
0349 ldloc.s local(0x0007)
034b callvirt 0x6000622 MyECIO.MyEcCtrl.Write
0350 call 0x600061b MyECIO.MyEcCtrl.get_Instance
0355 ldsfld 0x400280e 	..	
035a ldc.i4 128
035f ldelem.ref None
0360 ldsfld 0x400280e 	..	
0365 ldc.i4 121
036a ldelem.ref None
036b ldarg.0 None
036c callvirt 0x60020a5 	.

.Invoke
0371 callvirt 0x60020b3 	..Invoke
0376 ldc.i4 1899
037b ldloc.s local(0x0008)
037d callvirt 0x6000622 MyECIO.MyEcCtrl.Write
0382 ldsfld 0x400280e 	..	
0387 ldc.i4 79
038c ldelem.ref None
038d ldc.i4.6 None
038e newarr 0x1000006 System.String
0393 dup None
0394 ldc.i4.0 None
0395 ldstr '\x0e4\x18\x02\x1e\x1d\x19-�\ufff6NNT\x01\ufff5\uffefVO'
039a call 0x6001c6f 	.	.	
039f stelem.ref None
03a0 dup None
03a1 ldc.i4.1 None
03a2 ldsfld 0x400280e 	..	
03a7 ldc.i4 119
03ac ldelem.ref None
03ad ldloca.s local(0x0006)
03af callvirt 0x60020a1 	.
.Invoke
03b4 stelem.ref None
03b5 dup None
03b6 ldc.i4.2 None
03b7 ldstr 'k'
03bc call 0x6001c6f 	.	.	
03c1 stelem.ref None
03c2 dup None
03c3 ldc.i4.3 None
03c4 ldsfld 0x400280e 	..	
03c9 ldc.i4 119
03ce ldelem.ref None
03cf ldloca.s local(0x0007)
03d1 callvirt 0x60020a1 	.
.Invoke
03d6 stelem.ref None
03d7 dup None
03d8 ldc.i4.4 None
03d9 ldstr 'k'
03de call 0x6001c6f 	.	.	
03e3 stelem.ref None
03e4 dup None
03e5 ldc.i4.5 None
03e6 ldsfld 0x400280e 	..	
03eb ldc.i4 119
03f0 ldelem.ref None
03f1 ldloca.s local(0x0008)
03f3 callvirt 0x60020a1 	.
.Invoke
03f8 stelem.ref None
03f9 callvirt 0x6002051 	..Invoke
03fe call 0x60006e7 Utility.LogCtrl.Write
0403 br 2116
0408 ldarg.0 None
0409 ldflda 0x4001ff1 MyControlCenter.MyRgbKeyboard.HIDKeyboard._EffectData
040e ldfld 0x4000915 LightingModel.SAVE_LIGHTING_EFFECT_DATA.save_effect
0413 ldc.i4.2 None
0414 bne.un 1714
0419 ldc.i4.0 None
041a stloc.s local(0x0009)
041c call 0x600061b MyECIO.MyEcCtrl.get_Instance
0421 ldsfld 0x400280e 	..	
0426 ldc.i4 128
042b ldelem.ref None
042c ldsfld 0x400280e 	..	
0431 ldc.i4 121
0436 ldelem.ref None
0437 ldarg.0 None
0438 callvirt 0x60020a5 	.

.Invoke
043d callvirt 0x60020b3 	..Invoke
0442 ldc.i4 1895
0447 ldloca.s local(0x0009)
0449 callvirt 0x6000621 MyECIO.MyEcCtrl.Read
044e ldsfld 0x400280e 	..	
0453 ldc.i4 29
0458 ldelem.ref None
0459 ldstr 't+mkk_￼+\x19\x1c&\x11\x11\t\x11U\x02\x10\x13\rVO'
045e call 0x6001c6f 	.	.	
0463 ldsfld 0x400280e 	..	
0468 ldc.i4 119
046d ldelem.ref None
046e ldloca.s local(0x0009)
0470 callvirt 0x60020a1 	.
.Invoke
0475 callvirt 0x6001fed 	.	.Invoke
047a ldstr '\ufff6\x1c0\n&%!\x15\x05\ufffe'
047f call 0x6001c6f 	.	.	
0484 ldstr '73LF]g_FbRSLB0/8A(@;+^\'JHMRFB 9AF6J+\x1bN\x17:8=B62\x10)16&:\x1b\x0b>\x16"$\x0c\x1d8 $\x1d-\x16\r\x00\x00\ufff2\x00\x11,\x14\x18\x11!\n\x01\ufff4\ufff4￦\ufff4\x05 \x08\x0c\x05\x15\ufffeￇ\x03\x12'
0489 call 0x6001c6f 	.	.	
048e ldc.i4 1880
0493 call 0x60006ee Utility.LogCtrl.TraceMessage
0498 ldloc.s local(0x0009)
049a ldc.i4.s 127
049c and None
049d conv.u1 None
049e stloc.s local(0x0009)
04a0 call 0x600061b MyECIO.MyEcCtrl.get_Instance
04a5 ldsfld 0x400280e 	..	
04aa ldc.i4 128
04af ldelem.ref None
04b0 ldsfld 0x400280e 	..	
04b5 ldc.i4 121
04ba ldelem.ref None
04bb ldarg.0 None
04bc callvirt 0x60020a5 	.

.Invoke
04c1 callvirt 0x60020b3 	..Invoke
04c6 ldc.i4 1895
04cb ldloc.s local(0x0009)
04cd callvirt 0x6000622 MyECIO.MyEcCtrl.Write
04d2 ldsfld 0x400280e 	..	
04d7 ldc.i4 29
04dc ldelem.ref None
04dd ldstr 'u,llj`�,\x18\x1b\'\x12\x10\n\x10V\ufffe"\n\x1e\x0cVO'
04e2 call 0x6001c6f 	.	.	
04e7 ldsfld 0x400280e 	..	
04ec ldc.i4 119
04f1 ldelem.ref None
04f2 ldloca.s local(0x0009)
04f4 callvirt 0x60020a1 	.
.Invoke
04f9 callvirt 0x6001fed 	.	.Invoke
04fe ldstr '\ufff6\x1c0\n&%!\x15\x05\ufffe'
0503 call 0x6001c6f 	.	.	
0508 ldstr '73LF]g_FbRSLB0/8A(@;+^\'JHMRFB 9AF6J+\x1bN\x17:8=B62\x10)16&:\x1b\x0b>\x16"$\x0c\x1d8 $\x1d-\x16\r\x00\x00\ufff2\x00\x11,\x14\x18\x11!\n\x01\ufff4\ufff4￦\ufff4\x05 \x08\x0c\x05\x15\ufffeￇ\x03\x12'
050d call 0x6001c6f 	.	.	
0512 ldc.i4 1883
0517 call 0x60006ee Utility.LogCtrl.TraceMessage
051c ldc.i4.0 None
051d stloc.s local(0x000C)
051f ldc.i4.0 None
0520 stloc.s local(0x000D)
0522 ldarg.0 None
0523 ldflda 0x4001ff1 MyControlCenter.MyRgbKeyboard.HIDKeyboard._EffectData
0528 ldflda 0x4000919 LightingModel.SAVE_LIGHTING_EFFECT_DATA.save_layout_color
052d ldfld 0x400087b LightingModel.RGBKB_Color.ColorBuffer
0532 ldc.i4.0 None
0533 ldelema 0x20000d3 LightingModel.RGB_S
0538 ldfld 0x4000876 LightingModel.RGB_S.R
053d stloc.s local(0x000A)
053f ldarg.0 None
0540 ldflda 0x4001ff1 MyControlCenter.MyRgbKeyboard.HIDKeyboard._EffectData
0545 ldflda 0x4000919 LightingModel.SAVE_LIGHTING_EFFECT_DATA.save_layout_color
054a ldfld 0x400087b LightingModel.RGBKB_Color.ColorBuffer
054f ldc.i4.0 None
0550 ldelema 0x20000d3 LightingModel.RGB_S
0555 ldfld 0x4000877 LightingModel.RGB_S.G
055a stloc.s local(0x000B)
055c ldarg.0 None
055d ldflda 0x4001ff1 MyControlCenter.MyRgbKeyboard.HIDKeyboard._EffectData
0562 ldflda 0x4000919 LightingModel.SAVE_LIGHTING_EFFECT_DATA.save_layout_color
0567 ldfld 0x400087b LightingModel.RGBKB_Color.ColorBuffer
056c ldc.i4.0 None
056d ldelema 0x20000d3 LightingModel.RGB_S
0572 ldfld 0x4000878 LightingModel.RGB_S.B
0577 stloc.s local(0x000C)
0579 ldloc.s local(0x000A)
057b ldc.i4 255
0580 bne.un.s 1426
0582 ldloc.s local(0x000B)
0584 brtrue.s 1426
0586 ldloc.s local(0x000C)
0588 brtrue.s 1426
058a ldc.i4.2 None
058b stloc.s local(0x000D)
058d br 1587
0592 ldloc.s local(0x000A)
0594 brtrue.s 1451
0596 ldloc.s local(0x000B)
0598 ldc.i4 255
059d bne.un.s 1451
059f ldloc.s local(0x000C)
05a1 brtrue.s 1451
05a3 ldc.i4.3 None
05a4 stloc.s local(0x000D)
05a6 br 1587
05ab ldloc.s local(0x000A)
05ad brtrue.s 1476
05af ldloc.s local(0x000B)
05b1 brtrue.s 1476
05b3 ldloc.s local(0x000C)
05b5 ldc.i4 255
05ba bne.un.s 1476
05bc ldc.i4.4 None
05bd stloc.s local(0x000D)
05bf br 1587
05c4 ldloc.s local(0x000A)
05c6 ldc.i4 255
05cb bne.un.s 1503
05cd ldloc.s local(0x000B)
05cf ldc.i4 255
05d4 bne.un.s 1503
05d6 ldloc.s local(0x000C)
05d8 brtrue.s 1503
05da ldc.i4.5 None
05db stloc.s local(0x000D)
05dd br.s 1587
05df ldloc.s local(0x000A)
05e1 ldc.i4 136
05e6 bne.un.s 1530
05e8 ldloc.s local(0x000B)
05ea brtrue.s 1530
05ec ldloc.s local(0x000C)
05ee ldc.i4 255
05f3 bne.un.s 1530
05f5 ldc.i4.6 None
05f6 stloc.s local(0x000D)
05f8 br.s 1587
05fa ldloc.s local(0x000A)
05fc brtrue.s 1557
05fe ldloc.s local(0x000B)
0600 ldc.i4 255
0605 bne.un.s 1557
0607 ldloc.s local(0x000C)
0609 ldc.i4 255
060e bne.un.s 1557
0610 ldc.i4.7 None
0611 stloc.s local(0x000D)
0613 br.s 1587
0615 ldloc.s local(0x000A)
0617 ldc.i4 255
061c bne.un.s 1587
061e ldloc.s local(0x000B)
0620 ldc.i4 255
0625 bne.un.s 1587
0627 ldloc.s local(0x000C)
0629 ldc.i4 255
062e bne.un.s 1587
0630 ldc.i4.1 None
0631 stloc.s local(0x000D)
0633 ldc.i4.0 None
0634 stloc.s local(0x0009)
0636 call 0x600061b MyECIO.MyEcCtrl.get_Instance
063b ldsfld 0x400280e 	..	
0640 ldc.i4 128
0645 ldelem.ref None
0646 ldsfld 0x400280e 	..	
064b ldc.i4 121
0650 ldelem.ref None
0651 ldarg.0 None
0652 callvirt 0x60020a5 	.

.Invoke
0657 callvirt 0x60020b3 	..Invoke
065c ldc.i4 1989
0661 ldloca.s local(0x0009)
0663 callvirt 0x6000621 MyECIO.MyEcCtrl.Read
0668 ldloc.s local(0x0009)
066a ldc.i4 248
066f and None
0670 conv.u1 None
0671 stloc.s local(0x0009)
0673 ldloc.s local(0x0009)
0675 ldloc.s local(0x000D)
0677 or None
0678 conv.u1 None
0679 stloc.s local(0x0009)
067b call 0x600061b MyECIO.MyEcCtrl.get_Instance
0680 ldsfld 0x400280e 	..	
0685 ldc.i4 128
068a ldelem.ref None
068b ldsfld 0x400280e 	..	
0690 ldc.i4 121
0695 ldelem.ref None
0696 ldarg.0 None
0697 callvirt 0x60020a5 	.

.Invoke
069c callvirt 0x60020b3 	..Invoke
06a1 ldc.i4 1989
06a6 ldloc.s local(0x0009)
06a8 callvirt 0x6000622 MyECIO.MyEcCtrl.Write
06ad br 2116
06b2 ldarg.0 None
06b3 ldflda 0x4001ff1 MyControlCenter.MyRgbKeyboard.HIDKeyboard._EffectData
06b8 ldfld 0x4000915 LightingModel.SAVE_LIGHTING_EFFECT_DATA.save_effect
06bd ldc.i4.s 19
06bf bne.un 2116
06c4 ldc.i4.0 None
06c5 stloc.s local(0x000E)
06c7 call 0x600061b MyECIO.MyEcCtrl.get_Instance
06cc ldsfld 0x400280e 	..	
06d1 ldc.i4 128
06d6 ldelem.ref None
06d7 ldsfld 0x400280e 	..	
06dc ldc.i4 121
06e1 ldelem.ref None
06e2 ldarg.0 None
06e3 callvirt 0x60020a5 	.

.Invoke
06e8 callvirt 0x60020b3 	..Invoke
06ed ldc.i4 1989
06f2 ldloca.s local(0x000E)
06f4 callvirt 0x6000621 MyECIO.MyEcCtrl.Read
06f9 ldloc.s local(0x000E)
06fb ldc.i4 248
0700 and None
0701 conv.u1 None
0702 stloc.s local(0x000E)
0704 call 0x600061b MyECIO.MyEcCtrl.get_Instance
0709 ldsfld 0x400280e 	..	
070e ldc.i4 128
0713 ldelem.ref None
0714 ldsfld 0x400280e 	..	
0719 ldc.i4 121
071e ldelem.ref None
071f ldarg.0 None
0720 callvirt 0x60020a5 	.

.Invoke
0725 callvirt 0x60020b3 	..Invoke
072a ldc.i4 1989
072f ldloc.s local(0x000E)
0731 callvirt 0x6000622 MyECIO.MyEcCtrl.Write
0736 ldc.i4.0 None
0737 stloc.s local(0x000E)
0739 call 0x600061b MyECIO.MyEcCtrl.get_Instance
073e ldsfld 0x400280e 	..	
0743 ldc.i4 128
0748 ldelem.ref None
0749 ldsfld 0x400280e 	..	
074e ldc.i4 121
0753 ldelem.ref None
0754 ldarg.0 None
0755 callvirt 0x60020a5 	.

.Invoke
075a callvirt 0x60020b3 	..Invoke
075f ldc.i4 1895
0764 ldloca.s local(0x000E)
0766 callvirt 0x6000621 MyECIO.MyEcCtrl.Read
076b ldsfld 0x400280e 	..	
0770 ldc.i4 29
0775 ldelem.ref None
0776 ldstr 's*nnl^\ufffe\x11\x0f\x0f+\x16"\nU\x06\x0e\x11\rZO'
077b call 0x6001c6f 	.	.	
0780 ldsfld 0x400280e 	..	
0785 ldc.i4 119
078a ldelem.ref None
078b ldloca.s local(0x000E)
078d callvirt 0x60020a1 	.
.Invoke
0792 callvirt 0x6001fed 	.	.Invoke
0797 ldstr '\ufff6\x1c0\n&%!\x15\x05\ufffe'
079c call 0x6001c6f 	.	.	
07a1 ldstr '73LF]g_FbRSLB0/8A(@;+^\'JHMRFB 9AF6J+\x1bN\x17:8=B62\x10)16&:\x1b\x0b>\x16"$\x0c\x1d8 $\x1d-\x16\r\x00\x00\ufff2\x00\x11,\x14\x18\x11!\n\x01\ufff4\ufff4￦\ufff4\x05 \x08\x0c\x05\x15\ufffeￇ\x03\x12'
07a6 call 0x6001c6f 	.	.	
07ab ldc.i4 1922
07b0 call 0x60006ee Utility.LogCtrl.TraceMessage
07b5 ldloc.s local(0x000E)
07b7 ldc.i4.s 127
07b9 and None
07ba conv.u1 None
07bb stloc.s local(0x000E)
07bd ldloc.s local(0x000E)
07bf ldc.i4 128
07c4 or None
07c5 conv.u1 None
07c6 stloc.s local(0x000E)
07c8 call 0x600061b MyECIO.MyEcCtrl.get_Instance
07cd ldsfld 0x400280e 	..	
07d2 ldc.i4 128
07d7 ldelem.ref None
07d8 ldsfld 0x400280e 	..	
07dd ldc.i4 121
07e2 ldelem.ref None
07e3 ldarg.0 None
07e4 callvirt 0x60020a5 	.

.Invoke
07e9 callvirt 0x60020b3 	..Invoke
07ee ldc.i4 1895
07f3 ldloc.s local(0x000E)
07f5 callvirt 0x6000622 MyECIO.MyEcCtrl.Write
07fa ldsfld 0x400280e 	..	
07ff ldc.i4 29
0804 ldelem.ref None
0805 ldstr 't+mkk_�\x10\x10\x0e(\x13%\x0bV\x00"\x0c\x1e\x0eVO'
080a call 0x6001c6f 	.	.	
080f ldsfld 0x400280e 	..	
0814 ldc.i4 119
0819 ldelem.ref None
081a ldloca.s local(0x000E)
081c callvirt 0x60020a1 	.
.Invoke
0821 callvirt 0x6001fed 	.	.Invoke
0826 ldstr '\ufff6\x1c0\n&%!\x15\x05\ufffe'
082b call 0x6001c6f 	.	.	
0830 ldstr '73LF]g_FbRSLB0/8A(@;+^\'JHMRFB 9AF6J+\x1bN\x17:8=B62\x10)16&:\x1b\x0b>\x16"$\x0c\x1d8 $\x1d-\x16\r\x00\x00\ufff2\x00\x11,\x14\x18\x11!\n\x01\ufff4\ufff4￦\ufff4\x05 \x08\x0c\x05\x15\ufffeￇ\x03\x12'
0835 call 0x6001c6f 	.	.	
083a ldc.i4 1926
083f call 0x60006ee Utility.LogCtrl.TraceMessage
0844 call 0x600061b MyECIO.MyEcCtrl.get_Instance
0849 ldsfld 0x400280e 	..	
084e ldc.i4 128
0853 ldelem.ref None
0854 ldsfld 0x400280e 	..	
0859 ldc.i4 121
085e ldelem.ref None
085f ldarg.0 None
0860 callvirt 0x60020a5 	.

.Invoke
0865 callvirt 0x60020b3 	..Invoke
086a ldc.i4 1963
086f ldloca.s local(0x0001)
0871 callvirt 0x6000621 MyECIO.MyEcCtrl.Read
0876 ldloc.1 None
0877 ldc.i4.s 31
0879 and None
087a conv.u1 None
087b stloc.1 None
087c ldloc.0 None
087d brfalse.s 2177
087f ldc.i4.0 None
0880 stloc.2 None
0881 ldloc.1 None
0882 ldloc.2 None
0883 or None
0884 conv.u1 None
0885 stloc.1 None
0886 call 0x600061b MyECIO.MyEcCtrl.get_Instance
088b ldsfld 0x400280e 	..	
0890 ldc.i4 128
0895 ldelem.ref None
0896 ldsfld 0x400280e 	..	
089b ldc.i4 121
08a0 ldelem.ref None
08a1 ldarg.0 None
08a2 callvirt 0x60020a5 	.

.Invoke
08a7 callvirt 0x60020b3 	..Invoke
08ac ldc.i4 1963
08b1 ldloc.1 None
08b2 callvirt 0x6000622 MyECIO.MyEcCtrl.Write
08b7 ldsfld 0x400280e 	..	
08bc ldc.i4 29
08c1 ldelem.ref None
08c2 ldstr '\t)#\ufff7\x19\x18\x12$\ufff2\uffefUSK￬\x1b\x0f\x0c\x0e\x19\x12\x06\x13\x12Z?'
08c7 call 0x6001c6f 	.	.	
08cc ldsfld 0x400280e 	..	
08d1 ldc.i4 119
08d6 ldelem.ref None
08d7 ldloca.s local(0x0001)
08d9 callvirt 0x60020a1 	.
.Invoke
08de callvirt 0x6001fed 	.	.Invoke
08e3 call 0x60006e7 Utility.LogCtrl.Write
08e8 ldc.i4.0 None
08e9 stloc.3 None
08ea call 0x600061b MyECIO.MyEcCtrl.get_Instance
08ef ldsfld 0x400280e 	..	
08f4 ldc.i4 128
08f9 ldelem.ref None
08fa ldsfld 0x400280e 	..	
08ff ldc.i4 121
0904 ldelem.ref None
0905 ldarg.0 None
0906 callvirt 0x60020a5 	.

.Invoke
090b callvirt 0x60020b3 	..Invoke
0910 ldc.i4 1932
0915 ldloca.s local(0x0003)
0917 callvirt 0x6000621 MyECIO.MyEcCtrl.Read
091c ldloc.3 None
091d ldc.i4.s 16
091f or None
0920 conv.u1 None
0921 stloc.3 None
0922 call 0x600061b MyECIO.MyEcCtrl.get_Instance
0927 ldsfld 0x400280e 	..	
092c ldc.i4 128
0931 ldelem.ref None
0932 ldsfld 0x400280e 	..	
0937 ldc.i4 121
093c ldelem.ref None
093d ldarg.0 None
093e callvirt 0x60020a5 	.

.Invoke
0943 callvirt 0x60020b3 	..Invoke
0948 ldc.i4 1932
094d ldloc.3 None
094e callvirt 0x6000622 MyECIO.MyEcCtrl.Write
0953 br 2625
0958 ldc.i4.0 None
0959 stloc.s local(0x000F)
095b ldc.i4.0 None
095c stloc.s local(0x0010)
095e call 0x600061b MyECIO.MyEcCtrl.get_Instance
0963 ldsfld 0x400280e 	..	
0968 ldc.i4 128
096d ldelem.ref None
096e ldsfld 0x400280e 	..	
0973 ldc.i4 121
0978 ldelem.ref None
0979 ldarg.0 None
097a callvirt 0x60020a5 	.

.Invoke
097f callvirt 0x60020b3 	..Invoke
0984 ldc.i4 1963
0989 ldloca.s local(0x0010)
098b callvirt 0x6000621 MyECIO.MyEcCtrl.Read
0990 ldloc.s local(0x0010)
0992 ldc.i4.s 31
0994 and None
0995 conv.u1 None
0996 stloc.s local(0x0010)
0998 ldloc.s local(0x0010)
099a ldloc.s local(0x000F)
099c or None
099d conv.u1 None
099e stloc.s local(0x0010)
09a0 call 0x600061b MyECIO.MyEcCtrl.get_Instance
09a5 ldsfld 0x400280e 	..	
09aa ldc.i4 128
09af ldelem.ref None
09b0 ldsfld 0x400280e 	..	
09b5 ldc.i4 121
09ba ldelem.ref None
09bb ldarg.0 None
09bc callvirt 0x60020a5 	.

.Invoke
09c1 callvirt 0x60020b3 	..Invoke
09c6 ldc.i4 1963
09cb ldloc.s local(0x0010)
09cd callvirt 0x6000622 MyECIO.MyEcCtrl.Write
09d2 ldc.i4.0 None
09d3 stloc.s local(0x0011)
09d5 call 0x600061b MyECIO.MyEcCtrl.get_Instance
09da ldsfld 0x400280e 	..	
09df ldc.i4 128
09e4 ldelem.ref None
09e5 ldsfld 0x400280e 	..	
09ea ldc.i4 121
09ef ldelem.ref None
09f0 ldarg.0 None
09f1 callvirt 0x60020a5 	.

.Invoke
09f6 callvirt 0x60020b3 	..Invoke
09fb ldc.i4 1932
0a00 ldloca.s local(0x0011)
0a02 callvirt 0x6000621 MyECIO.MyEcCtrl.Read
0a07 ldloc.s local(0x0011)
0a09 ldc.i4.s 16
0a0b or None
0a0c conv.u1 None
0a0d stloc.s local(0x0011)
0a0f call 0x600061b MyECIO.MyEcCtrl.get_Instance
0a14 ldsfld 0x400280e 	..	
0a19 ldc.i4 128
0a1e ldelem.ref None
0a1f ldsfld 0x400280e 	..	
0a24 ldc.i4 121
0a29 ldelem.ref None
0a2a ldarg.0 None
0a2b callvirt 0x60020a5 	.

.Invoke
0a30 callvirt 0x60020b3 	..Invoke
0a35 ldc.i4 1932
0a3a ldloc.s local(0x0011)
0a3c callvirt 0x6000622 MyECIO.MyEcCtrl.Write
0a41 leave.s 2630
0a43 pop None
0a44 leave.s 2630
0a46 ret None

 0x60017c1 MyControlCenter.MyRgbKeyboard.HIDKeyboard.CloseEffectEC recovered
000c ldc.i4.0 None
000d stloc.0 None
000e ldc.i4.0 None
000f stloc.1 None
0010 call 0x600061b MyECIO.MyEcCtrl.get_Instance
0015 ldsfld 0x400280e 	..	
001a ldc.i4 128
001f ldelem.ref None
0020 ldsfld 0x400280e 	..	
0025 ldc.i4 121
002a ldelem.ref None
002b ldarg.0 None
002c callvirt 0x60020a5 	.

.Invoke
0031 callvirt 0x60020b3 	..Invoke
0036 ldc.i4 1963
003b ldloca.s local(0x0001)
003d callvirt 0x6000621 MyECIO.MyEcCtrl.Read
0042 ldloc.1 None
0043 ldc.i4.s 31
0045 and None
0046 conv.u1 None
0047 stloc.1 None
0048 ldloc.1 None
0049 ldloc.0 None
004a or None
004b conv.u1 None
004c stloc.1 None
004d call 0x600061b MyECIO.MyEcCtrl.get_Instance
0052 ldsfld 0x400280e 	..	
0057 ldc.i4 128
005c ldelem.ref None
005d ldsfld 0x400280e 	..	
0062 ldc.i4 121
0067 ldelem.ref None
0068 ldarg.0 None
0069 callvirt 0x60020a5 	.

.Invoke
006e callvirt 0x60020b3 	..Invoke
0073 ldc.i4 1963
0078 ldloc.1 None
0079 callvirt 0x6000622 MyECIO.MyEcCtrl.Write
007e ldc.i4.0 None
007f stloc.2 None
0080 call 0x600061b MyECIO.MyEcCtrl.get_Instance
0085 ldsfld 0x400280e 	..	
008a ldc.i4 128
008f ldelem.ref None
0090 ldsfld 0x400280e 	..	
0095 ldc.i4 121
009a ldelem.ref None
009b ldarg.0 None
009c callvirt 0x60020a5 	.

.Invoke
00a1 callvirt 0x60020b3 	..Invoke
00a6 ldc.i4 1932
00ab ldloca.s local(0x0002)
00ad callvirt 0x6000621 MyECIO.MyEcCtrl.Read
00b2 ldloc.2 None
00b3 ldc.i4.s 16
00b5 or None
00b6 conv.u1 None
00b7 stloc.2 None
00b8 call 0x600061b MyECIO.MyEcCtrl.get_Instance
00bd ldsfld 0x400280e 	..	
00c2 ldc.i4 128
00c7 ldelem.ref None
00c8 ldsfld 0x400280e 	..	
00cd ldc.i4 121
00d2 ldelem.ref None
00d3 ldarg.0 None
00d4 callvirt 0x60020a5 	.

.Invoke
00d9 callvirt 0x60020b3 	..Invoke
00de ldc.i4 1932
00e3 ldloc.2 None
00e4 callvirt 0x6000622 MyECIO.MyEcCtrl.Write
00e9 leave.s 238
00eb pop None
00ec leave.s 238
00ee ret None

 0x60017c2 MyControlCenter.MyRgbKeyboard.HIDKeyboard.DivideValue recovered
000c ldsfld 0x400280e 	..	
0011 ldc.i4 755
0016 ldelem.ref None
0017 ldsfld 0x400280e 	..	
001c ldc.i4 188
0021 ldelem.ref None
0022 ldarg.0 None
0023 conv.r8 None
0024 ldarg.1 None
0025 div None
0026 callvirt 0x600212b 	..Invoke
002b conv.i4 None
002c ldc.i4.s 50
002e callvirt 0x6002599 	.		.Invoke
0033 conv.u1 None
0034 ret None

 0x600080b MyRGBKeyboard.HIDLightbar_logo.RunEffctECALogo recovered
000c ldc.i4.s 64
000e stloc.0 None
000f ldc.i4.0 None
0010 stloc.1 None
0011 ldc.i4.3 None
0012 stloc.2 None
0013 ldarg.0 None
0014 ldfld 0x4000c78 MyRGBKeyboard.HIDLightbar_logo._PowerStatus
0019 ldc.i4.1 None
001a bne.un 3143
001f ldsfld 0x4000cc8 .<>o__38.<>p__1
0024 brtrue.s 93
0026 ldc.i4.0 None
0027 ldc.i4.s 83
0029 ldsfld 0x400280e 	..	
002e ldc.i4 32
0033 ldelem.ref None
0034 ldtoken 0x2000166 MyRGBKeyboard.HIDLightbar_logo
0039 callvirt 0x6001ff3 	..Invoke
003e ldc.i4.1 None
003f newarr 0x1000291 Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
0044 dup None
0045 ldc.i4.0 None
0046 ldc.i4.0 None
0047 ldnull None
0048 call 0xa000597 Create
004d stelem.ref None
004e call 0xa00059c UnaryOperation
0053 call 0xa000be4 Create
0058 stsfld 0x4000cc8 .<>o__38.<>p__1
005d ldsfld 0x4000cc8 .<>o__38.<>p__1
0062 ldfld 0xa000be5 Target
0067 ldsfld 0x4000cc8 .<>o__38.<>p__1
006c ldsfld 0x4000cc7 .<>o__38.<>p__0
0071 brtrue.s 180
0073 ldc.i4.0 None
0074 ldc.i4.s 13
0076 ldsfld 0x400280e 	..	
007b ldc.i4 32
0080 ldelem.ref None
0081 ldtoken 0x2000166 MyRGBKeyboard.HIDLightbar_logo
0086 callvirt 0x6001ff3 	..Invoke
008b ldc.i4.2 None
008c newarr 0x1000291 Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
0091 dup None
0092 ldc.i4.0 None
0093 ldc.i4.0 None
0094 ldnull None
0095 call 0xa000597 Create
009a stelem.ref None
009b dup None
009c ldc.i4.1 None
009d ldc.i4.2 None
009e ldnull None
009f call 0xa000597 Create
00a4 stelem.ref None
00a5 call 0xa00059d BinaryOperation
00aa call 0xa000c0f Create
00af stsfld 0x4000cc7 .<>o__38.<>p__0
00b4 ldsfld 0x4000cc7 .<>o__38.<>p__0
00b9 ldfld 0xa000c10 Target
00be ldsfld 0x4000cc7 .<>o__38.<>p__0
00c3 ldarg.1 None
00c4 ldnull None
00c5 callvirt 0xa000c11 Invoke
00ca callvirt 0xa000bec Invoke
00cf brfalse 565
00d4 ldsfld 0x400280e 	..	
00d9 ldc.i4 151
00de ldelem.ref None
00df ldc.i4 -2147483646
00e4 ldstr '\x10\ufffe\ufff7\ufffe\x0c\ufff2\x17\x1a\x1d\x15\r\ufff0\r\x11\x1a\n\x1aY\ufffe\ufff7￣￥￭\x02\x1d\x01\x05\x02\x0e\ufffb'
00e9 call 0x6001c6f 	.	.	
00ee ldstr '\ufff6\x1a\x1f\x17*\x1f\x1d-\x01\r\x0f\x16\r￼\ufff1\x11\x10\x0e\x13\x1b'
00f3 call 0x6001c6f 	.	.	
00f8 ldc.i4.0 None
00f9 box 0x1000012 System.Int32
00fe ldc.i4.4 None
00ff call 0x60006fe Utility.RegistryCtrl.RegistrySoftwareKeyRead
0104 unbox.any 0x1000012 System.Int32
0109 callvirt 0x60020e1 	..Invoke
010e stloc.1 None
010f ldc.i4 -2147483646
0114 ldstr '\x10\ufffe\ufff7\ufffe\x0c\ufff2\x17\x1a\x1d\x15\r\ufff0\r\x11\x1a\n\x1aY\ufffe\ufff7￣￥￭\x02\x1d\x01\x05\x02\x0e\ufffb'
0119 call 0x6001c6f 	.	.	
011e ldstr '\ufff7\x19\x1e\x18+ \x1c.\x00\x0e\x0e\x15\x0c\ufffb\ufff6\t\x07\x07#\ufff5\x01'
0123 call 0x6001c6f 	.	.	
0128 ldnull None
0129 ldc.i4.1 None
012a call 0x60006fe Utility.RegistryCtrl.RegistrySoftwareKeyRead
012f castclass 0x1000006 System.String
0134 stloc.s local(0x0008)
0136 ldsfld 0x400280e 	..	
013b ldc.i4 343
0140 ldelem.ref None
0141 ldloc.s local(0x0008)
0143 callvirt 0x6002261 	.
	.Invoke
0148 stloc.3 None
0149 ldc.i4 -2147483646
014e ldstr '\x10\ufffe\ufff7\ufffe\x0c\ufff2\x17\x1a\x1d\x15\r\ufff0\r\x11\x1a\n\x1aY\ufffe\ufff7￣￥￭\x02\x1d\x01\x05\x02\x0e\ufffb'
0153 call 0x6001c6f 	.	.	
0158 ldstr '\ufff7\x19\x1e\x18+ \x1c.\x00\x0e\x0e\x15\x0c\ufffb\ufff6\t\x07\x07#\ufff5￬'
015d call 0x6001c6f 	.	.	
0162 ldnull None
0163 ldc.i4.1 None
0164 call 0x60006fe Utility.RegistryCtrl.RegistrySoftwareKeyRead
0169 castclass 0x1000006 System.String
016e stloc.s local(0x0008)
0170 ldsfld 0x400280e 	..	
0175 ldc.i4 343
017a ldelem.ref None
017b ldloc.s local(0x0008)
017d callvirt 0x6002261 	.
	.Invoke
0182 stloc.s local(0x0004)
0184 ldc.i4 -2147483646
0189 ldstr '\x10\ufffe\ufff7\ufffe\x0c\ufff2\x17\x1a\x1d\x15\r\ufff0\r\x11\x1a\n\x1aY\ufffe\ufff7￣￥￭\x02\x1d\x01\x05\x02\x0e\ufffb'
018e call 0x6001c6f 	.	.	
0193 ldstr '\ufff7\x19\x1e\x18+ \x1c.\x00\x0e\x0e\x15\x0c\ufffb\ufff6\t\x07\x07#\ufff5\ufff1'
0198 call 0x6001c6f 	.	.	
019d ldnull None
019e ldc.i4.1 None
019f call 0x60006fe Utility.RegistryCtrl.RegistrySoftwareKeyRead
01a4 castclass 0x1000006 System.String
01a9 stloc.s local(0x0008)
01ab ldsfld 0x400280e 	..	
01b0 ldc.i4 343
01b5 ldelem.ref None
01b6 ldloc.s local(0x0008)
01b8 callvirt 0x6002261 	.
	.Invoke
01bd stloc.s local(0x0005)
01bf ldsfld 0x400280e 	..	
01c4 ldc.i4 341
01c9 ldelem.ref None
01ca ldarg.0 None
01cb ldfld 0x4000c71 MyRGBKeyboard.HIDLightbar_logo.ps
01d0 callvirt 0x600225d 	..Invoke
01d5 ldc.i4.1 None
01d6 bne.un.s 513
01d8 ldc.i4 -2147483646
01dd ldstr '\x10\ufffe\ufff7\ufffe\x0c\ufff2\x17\x1a\x1d\x15\r\ufff0\r\x11\x1a\n\x1aY\ufffe\ufff7￣￥￭\x02\x1d\x01\x05\x02\x0e\ufffb'
01e2 call 0x6001c6f 	.	.	
01e7 ldstr '\ufff7\x19\x1e\x18+ \x1c.\x00\x0e\x0e\x15\x0c\ufffb\ufff4\ufff5\uffe7\t\x0e\x08\x1b'
01ec call 0x6001c6f 	.	.	
01f1 ldnull None
01f2 ldc.i4.1 None
01f3 call 0x60006fe Utility.RegistryCtrl.RegistrySoftwareKeyRead
01f8 castclass 0x1000006 System.String
01fd stloc.s local(0x0008)
01ff br.s 552
0201 ldc.i4 -2147483646
0206 ldstr '\x10\ufffe\ufff7\ufffe\x0c\ufff2\x17\x1a\x1d\x15\r\ufff0\r\x11\x1a\n\x1aY\ufffe\ufff7￣￥￭\x02\x1d\x01\x05\x02\x0e\ufffb'
020b call 0x6001c6f 	.	.	
0210 ldstr '\ufff7\x19\x1e\x18+ \x1c.\x00\x0e\x0e\x15\x0c\ufffb\ufff1\ufff5\uffe7\t\x0e\x08\x1b'
0215 call 0x6001c6f 	.	.	
021a ldnull None
021b ldc.i4.1 None
021c call 0x60006fe Utility.RegistryCtrl.RegistrySoftwareKeyRead
0221 castclass 0x1000006 System.String
0226 stloc.s local(0x0008)
0228 ldloc.s local(0x0008)
022a call 0xa0000cc ToInt32
022f stloc.2 None
0230 br 1553
0235 ldsfld 0x4000ccb .<>o__38.<>p__4
023a brtrue.s 630
023c ldc.i4.0 None
023d ldsfld 0x400280e 	..	
0242 ldc.i4 32
0247 ldelem.ref None
0248 ldtoken 0x1000006 System.String
024d callvirt 0x6001ff3 	..Invoke
0252 ldsfld 0x400280e 	..	
0257 ldc.i4 32
025c ldelem.ref None
025d ldtoken 0x2000166 MyRGBKeyboard.HIDLightbar_logo
0262 callvirt 0x6001ff3 	..Invoke
0267 call 0xa00059a Convert
026c call 0xa000bf8 Create
0271 stsfld 0x4000ccb .<>o__38.<>p__4
0276 ldsfld 0x4000ccb .<>o__38.<>p__4
027b ldfld 0xa000bf9 Target
0280 ldsfld 0x4000ccb .<>o__38.<>p__4
0285 ldsfld 0x4000cc9 .<>o__38.<>p__2
028a brtrue.s 715
028c ldc.i4.0 None
028d ldsfld 0x400280e 	..	
0292 ldc.i4 32
0297 ldelem.ref None
0298 ldtoken 0x2000166 MyRGBKeyboard.HIDLightbar_logo
029d callvirt 0x6001ff3 	..Invoke
02a2 ldc.i4.2 None
02a3 newarr 0x1000291 Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
02a8 dup None
02a9 ldc.i4.0 None
02aa ldc.i4.0 None
02ab ldnull None
02ac call 0xa000597 Create
02b1 stelem.ref None
02b2 dup None
02b3 ldc.i4.1 None
02b4 ldc.i4.3 None
02b5 ldnull None
02b6 call 0xa000597 Create
02bb stelem.ref None
02bc call 0xa00059b GetIndex
02c1 call 0xa000be8 Create
02c6 stsfld 0x4000cc9 .<>o__38.<>p__2
02cb ldsfld 0x4000cc9 .<>o__38.<>p__2
02d0 ldfld 0xa000be9 Target
02d5 ldsfld 0x4000cc9 .<>o__38.<>p__2
02da ldarg.1 None
02db ldstr '1-,./\x1b'
02e0 call 0x6001c6f 	.	.	
02e5 callvirt 0xa000bea Invoke
02ea stloc.s local(0x000A)
02ec ldloc.s local(0x000A)
02ee brtrue.s 755
02f0 ldnull None
02f1 br.s 848
02f3 ldsfld 0x4000cca .<>o__38.<>p__3
02f8 brtrue.s 826
02fa ldc.i4.0 None
02fb ldstr '\ufffa4\ufff7\x17\x14*.&'
0300 call 0x6001c6f 	.	.	
0305 ldnull None
0306 ldsfld 0x400280e 	..	
030b ldc.i4 32
0310 ldelem.ref None
0311 ldtoken 0x2000166 MyRGBKeyboard.HIDLightbar_logo
0316 callvirt 0x6001ff3 	..Invoke
031b ldc.i4.1 None
031c newarr 0x1000291 Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
0321 dup None
0322 ldc.i4.0 None
0323 ldc.i4.0 None
0324 ldnull None
0325 call 0xa000597 Create
032a stelem.ref None
032b call 0xa000599 InvokeMember
0330 call 0xa000bb0 Create
0335 stsfld 0x4000cca .<>o__38.<>p__3
033a ldsfld 0x4000cca .<>o__38.<>p__3
033f ldfld 0xa000bb1 Target
0344 ldsfld 0x4000cca .<>o__38.<>p__3
0349 ldloc.s local(0x000A)
034b callvirt 0xa000bb2 Invoke
0350 callvirt 0xa000bfa Invoke
0355 stloc.s local(0x0009)
0357 ldsfld 0x400280e 	..	
035c ldc.i4 342
0361 ldelem.ref None
0362 ldsfld 0x400280e 	..	
0367 ldc.i4 32
036c ldelem.ref None
036d ldtoken 0x20000cf LightingModel.RGBKB_Effect
0372 callvirt 0x6001ff3 	..Invoke
0377 ldloc.s local(0x0009)
0379 callvirt 0x600225f 	.

.Invoke
037e unbox.any 0x20000cf LightingModel.RGBKB_Effect
0383 stloc.1 None
0384 ldsfld 0x4000cce .<>o__38.<>p__7
0389 brtrue.s 965
038b ldc.i4.0 None
038c ldsfld 0x400280e 	..	
0391 ldc.i4 32
0396 ldelem.ref None
0397 ldtoken 0x100003a System.UInt32
039c callvirt 0x6001ff3 	..Invoke
03a1 ldsfld 0x400280e 	..	
03a6 ldc.i4 32
03ab ldelem.ref None
03ac ldtoken 0x2000166 MyRGBKeyboard.HIDLightbar_logo
03b1 callvirt 0x6001ff3 	..Invoke
03b6 call 0xa00059a Convert
03bb call 0xa000bed Create
03c0 stsfld 0x4000cce .<>o__38.<>p__7
03c5 ldsfld 0x4000cce .<>o__38.<>p__7
03ca ldfld 0xa000bee Target
03cf ldsfld 0x4000cce .<>o__38.<>p__7
03d4 ldsfld 0x4000ccd .<>o__38.<>p__6
03d9 brtrue.s 1062
03db ldc.i4.0 None
03dc ldstr '\ufffa4\ufff9\x0c0\x15SQ'
03e1 call 0x6001c6f 	.	.	
03e6 ldnull None
03e7 ldsfld 0x400280e 	..	
03ec ldc.i4 32
03f1 ldelem.ref None
03f2 ldtoken 0x2000166 MyRGBKeyboard.HIDLightbar_logo
03f7 callvirt 0x6001ff3 	..Invoke
03fc ldc.i4.2 None
03fd newarr 0x1000291 Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
0402 dup None
0403 ldc.i4.0 None
0404 ldc.i4.s 33
0406 ldnull None
0407 call 0xa000597 Create
040c stelem.ref None
040d dup None
040e ldc.i4.1 None
040f ldc.i4.0 None
0410 ldnull None
0411 call 0xa000597 Create
0416 stelem.ref None
0417 call 0xa000599 InvokeMember
041c call 0xa000be6 Create
0421 stsfld 0x4000ccd .<>o__38.<>p__6
0426 ldsfld 0x4000ccd .<>o__38.<>p__6
042b ldfld 0xa000be7 Target
0430 ldsfld 0x4000ccd .<>o__38.<>p__6
0435 ldsfld 0x400280e 	..	
043a ldc.i4 32
043f ldelem.ref None
0440 ldtoken 0x1000026 System.Convert
0445 callvirt 0x6001ff3 	..Invoke
044a ldsfld 0x4000ccc .<>o__38.<>p__5
044f brtrue.s 1168
0451 ldc.i4.0 None
0452 ldsfld 0x400280e 	..	
0457 ldc.i4 32
045c ldelem.ref None
045d ldtoken 0x2000166 MyRGBKeyboard.HIDLightbar_logo
0462 callvirt 0x6001ff3 	..Invoke
0467 ldc.i4.2 None
0468 newarr 0x1000291 Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
046d dup None
046e ldc.i4.0 None
046f ldc.i4.0 None
0470 ldnull None
0471 call 0xa000597 Create
0476 stelem.ref None
0477 dup None
0478 ldc.i4.1 None
0479 ldc.i4.3 None
047a ldnull None
047b call 0xa000597 Create
0480 stelem.ref None
0481 call 0xa00059b GetIndex
0486 call 0xa000be8 Create
048b stsfld 0x4000ccc .<>o__38.<>p__5
0490 ldsfld 0x4000ccc .<>o__38.<>p__5
0495 ldfld 0xa000be9 Target
049a ldsfld 0x4000ccc .<>o__38.<>p__5
049f ldarg.1 None
04a0 ldstr "').(\x1b"
04a5 call 0x6001c6f 	.	.	
04aa callvirt 0xa000bea Invoke
04af callvirt 0xa000beb Invoke
04b4 callvirt 0xa000bef Invoke
04b9 stloc.2 None
04ba ldsfld 0x4000cd1 .<>o__38.<>p__10
04bf brtrue.s 1275
04c1 ldc.i4.0 None
04c2 ldsfld 0x400280e 	..	
04c7 ldc.i4 32
04cc ldelem.ref None
04cd ldtoken 0x20000d4 LightingModel.RGBKB_Color
04d2 callvirt 0x6001ff3 	..Invoke
04d7 ldsfld 0x400280e 	..	
04dc ldc.i4 32
04e1 ldelem.ref None
04e2 ldtoken 0x2000166 MyRGBKeyboard.HIDLightbar_logo
04e7 callvirt 0x6001ff3 	..Invoke
04ec call 0xa00059a Convert
04f1 call 0xa000c01 Create
04f6 stsfld 0x4000cd1 .<>o__38.<>p__10
04fb ldsfld 0x4000cd1 .<>o__38.<>p__10
0500 ldfld 0xa000c02 Target
0505 ldsfld 0x4000cd1 .<>o__38.<>p__10
050a ldsfld 0x4000cd0 .<>o__38.<>p__9
050f brtrue.s 1371
0511 ldc.i4.2 None
0512 ldstr '\x02\x15\x15,\x1c.+\ufff4*\r\r\x08\ufff2\ufff6e\x04￮\ufff2\ufff0\x03\x05\x01\x1d'
0517 call 0x6001c6f 	.	.	
051c ldnull None
051d ldsfld 0x400280e 	..	
0522 ldc.i4 32
0527 ldelem.ref None
0528 ldtoken 0x2000166 MyRGBKeyboard.HIDLightbar_logo
052d callvirt 0x6001ff3 	..Invoke
0532 ldc.i4.2 None
0533 newarr 0x1000291 Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
0538 dup None
0539 ldc.i4.0 None
053a ldc.i4.1 None
053b ldnull None
053c call 0xa000597 Create
0541 stelem.ref None
0542 dup None
0543 ldc.i4.1 None
0544 ldc.i4.0 None
0545 ldnull None
0546 call 0xa000597 Create
054b stelem.ref None
054c call 0xa000599 InvokeMember
0551 call 0xa000c12 Create
0556 stsfld 0x4000cd0 .<>o__38.<>p__9
055b ldsfld 0x4000cd0 .<>o__38.<>p__9
0560 ldfld 0xa000c13 Target
0565 ldsfld 0x4000cd0 .<>o__38.<>p__9
056a ldarg.0 None
056b ldsfld 0x4000ccf .<>o__38.<>p__8
0570 brtrue.s 1457
0572 ldc.i4.0 None
0573 ldsfld 0x400280e 	..	
0578 ldc.i4 32
057d ldelem.ref None
057e ldtoken 0x2000166 MyRGBKeyboard.HIDLightbar_logo
0583 callvirt 0x6001ff3 	..Invoke
0588 ldc.i4.2 None
0589 newarr 0x1000291 Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
058e dup None
058f ldc.i4.0 None
0590 ldc.i4.0 None
0591 ldnull None
0592 call 0xa000597 Create
0597 stelem.ref None
0598 dup None
0599 ldc.i4.1 None
059a ldc.i4.3 None
059b ldnull None
059c call 0xa000597 Create
05a1 stelem.ref None
05a2 call 0xa00059b GetIndex
05a7 call 0xa000be8 Create
05ac stsfld 0x4000ccf .<>o__38.<>p__8
05b1 ldsfld 0x4000ccf .<>o__38.<>p__8
05b6 ldfld 0xa000be9 Target
05bb ldsfld 0x4000ccf .<>o__38.<>p__8
05c0 ldarg.1 None
05c1 ldstr "4'%%!"
05c6 call 0x6001c6f 	.	.	
05cb callvirt 0xa000bea Invoke
05d0 callvirt 0xa000c14 Invoke
05d5 callvirt 0xa000c03 Invoke
05da dup None
05db ldfld 0x400087b LightingModel.RGBKB_Color.ColorBuffer
05e0 ldc.i4.0 None
05e1 ldelema 0x20000d3 LightingModel.RGB_S
05e6 ldfld 0x4000876 LightingModel.RGB_S.R
05eb stloc.3 None
05ec dup None
05ed ldfld 0x400087b LightingModel.RGBKB_Color.ColorBuffer
05f2 ldc.i4.0 None
05f3 ldelema 0x20000d3 LightingModel.RGB_S
05f8 ldfld 0x4000877 LightingModel.RGB_S.G
05fd stloc.s local(0x0004)
05ff ldfld 0x400087b LightingModel.RGBKB_Color.ColorBuffer
0604 ldc.i4.0 None
0605 ldelema 0x20000d3 LightingModel.RGB_S
060a ldfld 0x4000878 LightingModel.RGB_S.B
060f stloc.s local(0x0005)
0611 ldc.i4.0 None
0612 stloc.s local(0x0006)
0614 ldloc.2 None
0615 switch [1584, 1588, 1593, 1598, 1603]
062e br.s 1609
0630 ldc.i4.0 None
0631 stloc.0 None
0632 br.s 1609
0634 ldc.i4.s 32
0636 stloc.0 None
0637 br.s 1609
0639 ldc.i4.s 64
063b stloc.0 None
063c br.s 1609
063e ldc.i4.s 96
0640 stloc.0 None
0641 br.s 1609
0643 ldc.i4 128
0648 stloc.0 None
0649 ldloc.1 None
064a brtrue 2192
064f ldc.i4.0 None
0650 stloc.s local(0x000B)
0652 call 0x600061b MyECIO.MyEcCtrl.get_Instance
0657 ldsfld 0x400280e 	..	
065c ldc.i4 128
0661 ldelem.ref None
0662 ldsfld 0x400280e 	..	
0667 ldc.i4 121
066c ldelem.ref None
066d ldarg.0 None
066e callvirt 0x60020a5 	.

.Invoke
0673 callvirt 0x60020b3 	..Invoke
0678 ldc.i4 1895
067d ldloca.s local(0x000B)
067f callvirt 0x6000621 MyECIO.MyEcCtrl.Read
0684 ldloc.s local(0x000B)
0686 ldc.i4.s 127
0688 and None
0689 conv.u1 None
068a stloc.s local(0x000B)
068c call 0x600061b MyECIO.MyEcCtrl.get_Instance
0691 ldsfld 0x400280e 	..	
0696 ldc.i4 128
069b ldelem.ref None
069c ldsfld 0x400280e 	..	
06a1 ldc.i4 121
06a6 ldelem.ref None
06a7 ldarg.0 None
06a8 callvirt 0x60020a5 	.

.Invoke
06ad callvirt 0x60020b3 	..Invoke
06b2 ldc.i4 1895
06b7 ldloc.s local(0x000B)
06b9 callvirt 0x6000622 MyECIO.MyEcCtrl.Write
06be ldc.i4.0 None
06bf stloc.s local(0x000B)
06c1 call 0x600061b MyECIO.MyEcCtrl.get_Instance
06c6 ldsfld 0x400280e 	..	
06cb ldc.i4 128
06d0 ldelem.ref None
06d1 ldsfld 0x400280e 	..	
06d6 ldc.i4 121
06db ldelem.ref None
06dc ldarg.0 None
06dd callvirt 0x60020a5 	.

.Invoke
06e2 callvirt 0x60020b3 	..Invoke
06e7 ldc.i4 1989
06ec ldloca.s local(0x000B)
06ee callvirt 0x6000621 MyECIO.MyEcCtrl.Read
06f3 ldloc.s local(0x000B)
06f5 ldc.i4 248
06fa and None
06fb conv.u1 None
06fc stloc.s local(0x000B)
06fe call 0x600061b MyECIO.MyEcCtrl.get_Instance
0703 ldsfld 0x400280e 	..	
0708 ldc.i4 128
070d ldelem.ref None
070e ldsfld 0x400280e 	..	
0713 ldc.i4 121
0718 ldelem.ref None
0719 ldarg.0 None
071a callvirt 0x60020a5 	.

.Invoke
071f callvirt 0x60020b3 	..Invoke
0724 ldc.i4 1989
0729 ldloc.s local(0x000B)
072b callvirt 0x6000622 MyECIO.MyEcCtrl.Write
0730 ldloc.3 None
0731 ldc.r8 5.1
073a call 0x60017c2 MyControlCenter.MyRgbKeyboard.HIDKeyboard.DivideValue
073f stloc.s local(0x000C)
0741 ldloc.s local(0x0004)
0743 ldc.r8 5.1
074c call 0x60017c2 MyControlCenter.MyRgbKeyboard.HIDKeyboard.DivideValue
0751 stloc.s local(0x000D)
0753 ldloc.s local(0x0005)
0755 ldc.r8 5.1
075e call 0x60017c2 MyControlCenter.MyRgbKeyboard.HIDKeyboard.DivideValue
0763 stloc.s local(0x000E)
0765 ldloc.s local(0x000C)
0767 brtrue.s 1908
0769 ldloc.s local(0x000D)
076b brtrue.s 1908
076d ldloc.s local(0x000E)
076f brfalse 2863
0774 call 0x600061b MyECIO.MyEcCtrl.get_Instance
0779 ldsfld 0x400280e 	..	
077e ldc.i4 128
0783 ldelem.ref None
0784 ldsfld 0x400280e 	..	
0789 ldc.i4 121
078e ldelem.ref None
078f ldarg.0 None
0790 callvirt 0x60020a5 	.

.Invoke
0795 callvirt 0x60020b3 	..Invoke
079a ldc.i4 1897
079f ldloc.s local(0x000C)
07a1 callvirt 0x6000622 MyECIO.MyEcCtrl.Write
07a6 call 0x600061b MyECIO.MyEcCtrl.get_Instance
07ab ldsfld 0x400280e 	..	
07b0 ldc.i4 128
07b5 ldelem.ref None
07b6 ldsfld 0x400280e 	..	
07bb ldc.i4 121
07c0 ldelem.ref None
07c1 ldarg.0 None
07c2 callvirt 0x60020a5 	.

.Invoke
07c7 callvirt 0x60020b3 	..Invoke
07cc ldc.i4 1898
07d1 ldloc.s local(0x000D)
07d3 callvirt 0x6000622 MyECIO.MyEcCtrl.Write
07d8 call 0x600061b MyECIO.MyEcCtrl.get_Instance
07dd ldsfld 0x400280e 	..	
07e2 ldc.i4 128
07e7 ldelem.ref None
07e8 ldsfld 0x400280e 	..	
07ed ldc.i4 121
07f2 ldelem.ref None
07f3 ldarg.0 None
07f4 callvirt 0x60020a5 	.

.Invoke
07f9 callvirt 0x60020b3 	..Invoke
07fe ldc.i4 1899
0803 ldloc.s local(0x000E)
0805 callvirt 0x6000622 MyECIO.MyEcCtrl.Write
080a ldsfld 0x400280e 	..	
080f ldc.i4 79
0814 ldelem.ref None
0815 ldc.i4.6 None
0816 newarr 0x1000006 System.String
081b dup None
081c ldc.i4.0 None
081d ldstr '\x0e4\x18\x02\x1e\x1d\x19-�\ufff6NNT\x01\ufff5\uffefVO'
0822 call 0x6001c6f 	.	.	
0827 stelem.ref None
0828 dup None
0829 ldc.i4.1 None
082a ldsfld 0x400280e 	..	
082f ldc.i4 119
0834 ldelem.ref None
0835 ldloca.s local(0x000C)
0837 callvirt 0x60020a1 	.
.Invoke
083c stelem.ref None
083d dup None
083e ldc.i4.2 None
083f ldstr 'k'
0844 call 0x6001c6f 	.	.	
0849 stelem.ref None
084a dup None
084b ldc.i4.3 None
084c ldsfld 0x400280e 	..	
0851 ldc.i4 119
0856 ldelem.ref None
0857 ldloca.s local(0x000D)
0859 callvirt 0x60020a1 	.
.Invoke
085e stelem.ref None
085f dup None
0860 ldc.i4.4 None
0861 ldstr 'k'
0866 call 0x6001c6f 	.	.	
086b stelem.ref None
086c dup None
086d ldc.i4.5 None
086e ldsfld 0x400280e 	..	
0873 ldc.i4 119
0878 ldelem.ref None
0879 ldloca.s local(0x000E)
087b callvirt 0x60020a1 	.
.Invoke
0880 stelem.ref None
0881 callvirt 0x6002051 	..Invoke
0886 call 0x60006e7 Utility.LogCtrl.Write
088b br 2863
0890 ldloc.1 None
0891 ldc.i4.1 None
0892 bne.un 2619
0897 ldc.i4.0 None
0898 stloc.s local(0x000F)
089a call 0x600061b MyECIO.MyEcCtrl.get_Instance
089f ldsfld 0x400280e 	..	
08a4 ldc.i4 128
08a9 ldelem.ref None
08aa ldsfld 0x400280e 	..	
08af ldc.i4 121
08b4 ldelem.ref None
08b5 ldarg.0 None
08b6 callvirt 0x60020a5 	.

.Invoke
08bb callvirt 0x60020b3 	..Invoke
08c0 ldc.i4 1895
08c5 ldloca.s local(0x000F)
08c7 callvirt 0x6000621 MyECIO.MyEcCtrl.Read
08cc ldloc.s local(0x000F)
08ce ldc.i4.s 127
08d0 and None
08d1 conv.u1 None
08d2 stloc.s local(0x000F)
08d4 call 0x600061b MyECIO.MyEcCtrl.get_Instance
08d9 ldsfld 0x400280e 	..	
08de ldc.i4 128
08e3 ldelem.ref None
08e4 ldsfld 0x400280e 	..	
08e9 ldc.i4 121
08ee ldelem.ref None
08ef ldarg.0 None
08f0 callvirt 0x60020a5 	.

.Invoke
08f5 callvirt 0x60020b3 	..Invoke
08fa ldc.i4 1895
08ff ldloc.s local(0x000F)
0901 callvirt 0x6000622 MyECIO.MyEcCtrl.Write
0906 ldc.i4.0 None
0907 stloc.s local(0x0010)
0909 ldloc.3 None
090a ldc.i4 255
090f bne.un.s 2337
0911 ldloc.s local(0x0004)
0913 brtrue.s 2337
0915 ldloc.s local(0x0005)
0917 brtrue.s 2337
0919 ldc.i4.2 None
091a stloc.s local(0x0010)
091c br 2492
0921 ldloc.3 None
0922 brtrue.s 2361
0924 ldloc.s local(0x0004)
0926 ldc.i4 255
092b bne.un.s 2361
092d ldloc.s local(0x0005)
092f brtrue.s 2361
0931 ldc.i4.3 None
0932 stloc.s local(0x0010)
0934 br 2492
0939 ldloc.3 None
093a brtrue.s 2385
093c ldloc.s local(0x0004)
093e brtrue.s 2385
0940 ldloc.s local(0x0005)
0942 ldc.i4 255
0947 bne.un.s 2385
0949 ldc.i4.4 None
094a stloc.s local(0x0010)
094c br 2492
0951 ldloc.3 None
0952 ldc.i4 255
0957 bne.un.s 2411
0959 ldloc.s local(0x0004)
095b ldc.i4 255
0960 bne.un.s 2411
0962 ldloc.s local(0x0005)
0964 brtrue.s 2411
0966 ldc.i4.5 None
0967 stloc.s local(0x0010)
0969 br.s 2492
096b ldloc.3 None
096c ldc.i4 136
0971 bne.un.s 2437
0973 ldloc.s local(0x0004)
0975 brtrue.s 2437
0977 ldloc.s local(0x0005)
0979 ldc.i4 255
097e bne.un.s 2437
0980 ldc.i4.6 None
0981 stloc.s local(0x0010)
0983 br.s 2492
0985 ldloc.3 None
0986 brtrue.s 2463
0988 ldloc.s local(0x0004)
098a ldc.i4 255
098f bne.un.s 2463
0991 ldloc.s local(0x0005)
0993 ldc.i4 255
0998 bne.un.s 2463
099a ldc.i4.7 None
099b stloc.s local(0x0010)
099d br.s 2492
099f ldloc.3 None
09a0 ldc.i4 255
09a5 bne.un.s 2492
09a7 ldloc.s local(0x0004)
09a9 ldc.i4 255
09ae bne.un.s 2492
09b0 ldloc.s local(0x0005)
09b2 ldc.i4 255
09b7 bne.un.s 2492
09b9 ldc.i4.1 None
09ba stloc.s local(0x0010)
09bc ldc.i4.0 None
09bd stloc.s local(0x000F)
09bf call 0x600061b MyECIO.MyEcCtrl.get_Instance
09c4 ldsfld 0x400280e 	..	
09c9 ldc.i4 128
09ce ldelem.ref None
09cf ldsfld 0x400280e 	..	
09d4 ldc.i4 121
09d9 ldelem.ref None
09da ldarg.0 None
09db callvirt 0x60020a5 	.

.Invoke
09e0 callvirt 0x60020b3 	..Invoke
09e5 ldc.i4 1989
09ea ldloca.s local(0x000F)
09ec callvirt 0x6000621 MyECIO.MyEcCtrl.Read
09f1 ldloc.s local(0x000F)
09f3 ldc.i4 248
09f8 and None
09f9 conv.u1 None
09fa stloc.s local(0x000F)
09fc ldloc.s local(0x000F)
09fe ldloc.s local(0x0010)
0a00 or None
0a01 conv.u1 None
0a02 stloc.s local(0x000F)
0a04 call 0x600061b MyECIO.MyEcCtrl.get_Instance
0a09 ldsfld 0x400280e 	..	
0a0e ldc.i4 128
0a13 ldelem.ref None
0a14 ldsfld 0x400280e 	..	
0a19 ldc.i4 121
0a1e ldelem.ref None
0a1f ldarg.0 None
0a20 callvirt 0x60020a5 	.

.Invoke
0a25 callvirt 0x60020b3 	..Invoke
0a2a ldc.i4 1989
0a2f ldloc.s local(0x000F)
0a31 callvirt 0x6000622 MyECIO.MyEcCtrl.Write
0a36 br 2863
0a3b ldloc.1 None
0a3c ldc.i4.s 17
0a3e bne.un 2863
0a43 ldc.i4.0 None
0a44 stloc.s local(0x0011)
0a46 call 0x600061b MyECIO.MyEcCtrl.get_Instance
0a4b ldsfld 0x400280e 	..	
0a50 ldc.i4 128
0a55 ldelem.ref None
0a56 ldsfld 0x400280e 	..	
0a5b ldc.i4 121
0a60 ldelem.ref None
0a61 ldarg.0 None
0a62 callvirt 0x60020a5 	.

.Invoke
0a67 callvirt 0x60020b3 	..Invoke
0a6c ldc.i4 1989
0a71 ldloca.s local(0x0011)
0a73 callvirt 0x6000621 MyECIO.MyEcCtrl.Read
0a78 ldloc.s local(0x0011)
0a7a ldc.i4 248
0a7f and None
0a80 conv.u1 None
0a81 stloc.s local(0x0011)
0a83 call 0x600061b MyECIO.MyEcCtrl.get_Instance
0a88 ldsfld 0x400280e 	..	
0a8d ldc.i4 128
0a92 ldelem.ref None
0a93 ldsfld 0x400280e 	..	
0a98 ldc.i4 121
0a9d ldelem.ref None
0a9e ldarg.0 None
0a9f callvirt 0x60020a5 	.

.Invoke
0aa4 callvirt 0x60020b3 	..Invoke
0aa9 ldc.i4 1989
0aae ldloc.s local(0x0011)
0ab0 callvirt 0x6000622 MyECIO.MyEcCtrl.Write
0ab5 ldc.i4.0 None
0ab6 stloc.s local(0x0011)
0ab8 call 0x600061b MyECIO.MyEcCtrl.get_Instance
0abd ldsfld 0x400280e 	..	
0ac2 ldc.i4 128
0ac7 ldelem.ref None
0ac8 ldsfld 0x400280e 	..	
0acd ldc.i4 121
0ad2 ldelem.ref None
0ad3 ldarg.0 None
0ad4 callvirt 0x60020a5 	.

.Invoke
0ad9 callvirt 0x60020b3 	..Invoke
0ade ldc.i4 1895
0ae3 ldloca.s local(0x0011)
0ae5 callvirt 0x6000621 MyECIO.MyEcCtrl.Read
0aea ldloc.s local(0x0011)
0aec ldc.i4.s 127
0aee and None
0aef conv.u1 None
0af0 stloc.s local(0x0011)
0af2 ldloc.s local(0x0011)
0af4 ldc.i4 128
0af9 or None
0afa conv.u1 None
0afb stloc.s local(0x0011)
0afd call 0x600061b MyECIO.MyEcCtrl.get_Instance
0b02 ldsfld 0x400280e 	..	
0b07 ldc.i4 128
0b0c ldelem.ref None
0b0d ldsfld 0x400280e 	..	
0b12 ldc.i4 121
0b17 ldelem.ref None
0b18 ldarg.0 None
0b19 callvirt 0x60020a5 	.

.Invoke
0b1e callvirt 0x60020b3 	..Invoke
0b23 ldc.i4 1895
0b28 ldloc.s local(0x0011)
0b2a callvirt 0x6000622 MyECIO.MyEcCtrl.Write
0b2f call 0x600061b MyECIO.MyEcCtrl.get_Instance
0b34 ldsfld 0x400280e 	..	
0b39 ldc.i4 128
0b3e ldelem.ref None
0b3f ldsfld 0x400280e 	..	
0b44 ldc.i4 121
0b49 ldelem.ref None
0b4a ldarg.0 None
0b4b callvirt 0x60020a5 	.

.Invoke
0b50 callvirt 0x60020b3 	..Invoke
0b55 ldc.i4 1963
0b5a ldloca.s local(0x0006)
0b5c callvirt 0x6000621 MyECIO.MyEcCtrl.Read
0b61 ldloc.s local(0x0006)
0b63 ldc.i4.s 31
0b65 and None
0b66 conv.u1 None
0b67 stloc.s local(0x0006)
0b69 ldloc.s local(0x0006)
0b6b ldloc.0 None
0b6c or None
0b6d conv.u1 None
0b6e stloc.s local(0x0006)
0b70 call 0x600061b MyECIO.MyEcCtrl.get_Instance
0b75 ldsfld 0x400280e 	..	
0b7a ldc.i4 128
0b7f ldelem.ref None
0b80 ldsfld 0x400280e 	..	
0b85 ldc.i4 121
0b8a ldelem.ref None
0b8b ldarg.0 None
0b8c callvirt 0x60020a5 	.

.Invoke
0b91 callvirt 0x60020b3 	..Invoke
0b96 ldc.i4 1963
0b9b ldloc.s local(0x0006)
0b9d callvirt 0x6000622 MyECIO.MyEcCtrl.Write
0ba2 ldsfld 0x400280e 	..	
0ba7 ldc.i4 29
0bac ldelem.ref None
0bad ldstr '\t)#\ufff7\x19\x18\x12$\ufff2\uffefUSK￬\x1b\x0f\x0c\x0e\x19\x12\x06\x13\x12Z?'
0bb2 call 0x6001c6f 	.	.	
0bb7 ldsfld 0x400280e 	..	
0bbc ldc.i4 119
0bc1 ldelem.ref None
0bc2 ldloca.s local(0x0006)
0bc4 callvirt 0x60020a1 	.
.Invoke
0bc9 callvirt 0x6001fed 	.	.Invoke
0bce call 0x60006e7 Utility.LogCtrl.Write
0bd3 ldc.i4.0 None
0bd4 stloc.s local(0x0007)
0bd6 call 0x600061b MyECIO.MyEcCtrl.get_Instance
0bdb ldsfld 0x400280e 	..	
0be0 ldc.i4 128
0be5 ldelem.ref None
0be6 ldsfld 0x400280e 	..	
0beb ldc.i4 121
0bf0 ldelem.ref None
0bf1 ldarg.0 None
0bf2 callvirt 0x60020a5 	.

.Invoke
0bf7 callvirt 0x60020b3 	..Invoke
0bfc ldc.i4 1932
0c01 ldloca.s local(0x0007)
0c03 callvirt 0x6000621 MyECIO.MyEcCtrl.Read
0c08 ldloc.s local(0x0007)
0c0a ldc.i4.s 16
0c0c or None
0c0d conv.u1 None
0c0e stloc.s local(0x0007)
0c10 call 0x600061b MyECIO.MyEcCtrl.get_Instance
0c15 ldsfld 0x400280e 	..	
0c1a ldc.i4 128
0c1f ldelem.ref None
0c20 ldsfld 0x400280e 	..	
0c25 ldc.i4 121
0c2a ldelem.ref None
0c2b ldarg.0 None
0c2c callvirt 0x60020a5 	.

.Invoke
0c31 callvirt 0x60020b3 	..Invoke
0c36 ldc.i4 1932
0c3b ldloc.s local(0x0007)
0c3d callvirt 0x6000622 MyECIO.MyEcCtrl.Write
0c42 br 3499
0c47 ldc.i4.0 None
0c48 stloc.0 None
0c49 ldstr '_'
0c4e call 0x6001c6f 	.	.	
0c53 stloc.s local(0x0012)
0c55 ldsfld 0x400280e 	..	
0c5a ldc.i4 341
0c5f ldelem.ref None
0c60 ldarg.0 None
0c61 ldfld 0x4000c71 MyRGBKeyboard.HIDLightbar_logo.ps
0c66 callvirt 0x600225d 	..Invoke
0c6b ldc.i4.1 None
0c6c bne.un.s 3223
0c6e ldc.i4 -2147483646
0c73 ldstr '\x10\ufffe\ufff7\ufffe\x0c\ufff2\x17\x1a\x1d\x15\r\ufff0\r\x11\x1a\n\x1aY\ufffe\ufff7￣￥￭\x02\x1d\x01\x05\x02\x0e\ufffb'
0c78 call 0x6001c6f 	.	.	
0c7d ldstr '\ufff7\x19\x1e\x18+ \x1c.\x00\x0e\x0e\x15\x0c\ufffb\ufff4\ufff5\uffe7\t\x0e\x08\x1b'
0c82 call 0x6001c6f 	.	.	
0c87 ldnull None
0c88 ldc.i4.1 None
0c89 call 0x60006fe Utility.RegistryCtrl.RegistrySoftwareKeyRead
0c8e castclass 0x1000006 System.String
0c93 stloc.s local(0x0012)
0c95 br.s 3262
0c97 ldc.i4 -2147483646
0c9c ldstr '\x10\ufffe\ufff7\ufffe\x0c\ufff2\x17\x1a\x1d\x15\r\ufff0\r\x11\x1a\n\x1aY\ufffe\ufff7￣￥￭\x02\x1d\x01\x05\x02\x0e\ufffb'
0ca1 call 0x6001c6f 	.	.	
0ca6 ldstr '\ufff7\x19\x1e\x18+ \x1c.\x00\x0e\x0e\x15\x0c\ufffb\ufff1\ufff5\uffe7\t\x0e\x08\x1b'
0cab call 0x6001c6f 	.	.	
0cb0 ldnull None
0cb1 ldc.i4.1 None
0cb2 call 0x60006fe Utility.RegistryCtrl.RegistrySoftwareKeyRead
0cb7 castclass 0x1000006 System.String
0cbc stloc.s local(0x0012)
0cbe ldloc.s local(0x0012)
0cc0 call 0xa0000cc ToInt32
0cc5 stloc.2 None
0cc6 ldc.i4.0 None
0cc7 stloc.s local(0x0013)
0cc9 call 0x600061b MyECIO.MyEcCtrl.get_Instance
0cce ldsfld 0x400280e 	..	
0cd3 ldc.i4 128
0cd8 ldelem.ref None
0cd9 ldsfld 0x400280e 	..	
0cde ldc.i4 121
0ce3 ldelem.ref None
0ce4 ldarg.0 None
0ce5 callvirt 0x60020a5 	.

.Invoke
0cea callvirt 0x60020b3 	..Invoke
0cef ldc.i4 1963
0cf4 ldloca.s local(0x0013)
0cf6 callvirt 0x6000621 MyECIO.MyEcCtrl.Read
0cfb ldloc.s local(0x0013)
0cfd ldc.i4.s 31
0cff and None
0d00 conv.u1 None
0d01 stloc.s local(0x0013)
0d03 ldloc.s local(0x0013)
0d05 ldloc.0 None
0d06 or None
0d07 conv.u1 None
0d08 stloc.s local(0x0013)
0d0a call 0x600061b MyECIO.MyEcCtrl.get_Instance
0d0f ldsfld 0x400280e 	..	
0d14 ldc.i4 128
0d19 ldelem.ref None
0d1a ldsfld 0x400280e 	..	
0d1f ldc.i4 121
0d24 ldelem.ref None
0d25 ldarg.0 None
0d26 callvirt 0x60020a5 	.

.Invoke
0d2b callvirt 0x60020b3 	..Invoke
0d30 ldc.i4 1963
0d35 ldloc.s local(0x0013)
0d37 callvirt 0x6000622 MyECIO.MyEcCtrl.Write
0d3c ldc.i4.0 None
0d3d stloc.s local(0x0014)
0d3f call 0x600061b MyECIO.MyEcCtrl.get_Instance
0d44 ldsfld 0x400280e 	..	
0d49 ldc.i4 128
0d4e ldelem.ref None
0d4f ldsfld 0x400280e 	..	
0d54 ldc.i4 121
0d59 ldelem.ref None
0d5a ldarg.0 None
0d5b callvirt 0x60020a5 	.

.Invoke
0d60 callvirt 0x60020b3 	..Invoke
0d65 ldc.i4 1932
0d6a ldloca.s local(0x0014)
0d6c callvirt 0x6000621 MyECIO.MyEcCtrl.Read
0d71 ldloc.s local(0x0014)
0d73 ldc.i4.s 16
0d75 or None
0d76 conv.u1 None
0d77 stloc.s local(0x0014)
0d79 call 0x600061b MyECIO.MyEcCtrl.get_Instance
0d7e ldsfld 0x400280e 	..	
0d83 ldc.i4 128
0d88 ldelem.ref None
0d89 ldsfld 0x400280e 	..	
0d8e ldc.i4 121
0d93 ldelem.ref None
0d94 ldarg.0 None
0d95 callvirt 0x60020a5 	.

.Invoke
0d9a callvirt 0x60020b3 	..Invoke
0d9f ldc.i4 1932
0da4 ldloc.s local(0x0014)
0da6 callvirt 0x6000622 MyECIO.MyEcCtrl.Write
0dab ldsfld 0x4000c76 MyRGBKeyboard.HIDLightbar_logo._HidLightbarStatus
0db0 ldsfld 0x400280e 	..	
0db5 ldc.i4 78
0dba ldelem.ref None
0dbb ldloca.s local(0x0002)
0dbd callvirt 0x600204f 	.	.Invoke
0dc2 stfld 0x4000c66 MyRGBKeyboard.HIDLightbar_logoStauts.brightNess
0dc7 ldsfld 0x4000c76 MyRGBKeyboard.HIDLightbar_logo._HidLightbarStatus
0dcc ldarg.0 None
0dcd ldflda 0x4000c78 MyRGBKeyboard.HIDLightbar_logo._PowerStatus
0dd2 constrained. 0x20000cb LightingModel.RGBKB_PowerStatus
0dd8 callvirt 0xa000002 ToString
0ddd stfld 0x4000c67 MyRGBKeyboard.HIDLightbar_logoStauts.powerStatus
0de2 ldsfld 0x4000c76 MyRGBKeyboard.HIDLightbar_logo._HidLightbarStatus
0de7 ldsfld 0x400280e 	..	
0dec ldc.i4 341
0df1 ldelem.ref None
0df2 ldarg.0 None
0df3 ldfld 0x4000c71 MyRGBKeyboard.HIDLightbar_logo.ps
0df8 callvirt 0x600225d 	..Invoke
0dfd brfalse.s 3601
0dff ldc.i4.1 None
0e00 stloc.s local(0x0015)
0e02 ldloca.s local(0x0015)
0e04 constrained. 0x20000ca LightingModel.RGBKB_PowerMode
0e0a callvirt 0xa000002 ToString
0e0f br.s 3617
0e11 ldc.i4.2 None
0e12 stloc.s local(0x0015)
0e14 ldloca.s local(0x0015)
0e16 constrained. 0x20000ca LightingModel.RGBKB_PowerMode
0e1c callvirt 0xa000002 ToString
0e21 stfld 0x4000c6a MyRGBKeyboard.HIDLightbar_logoStauts.currentPowerMode
0e26 ldsfld 0x4000c6c MyRGBKeyboard.HIDLightbar_logo.m_MQTTClient
0e2b ldstr '�\x1b\x1f\ufff6\x18\x19\x17*\x1b\x1b)\ufffb￭\t\x10\x07F\x01\x1f\x11\x1d\x1b\x1c'
0e30 call 0x6001c6f 	.	.	
0e35 ldsfld 0x4000c76 MyRGBKeyboard.HIDLightbar_logo._HidLightbarStatus
0e3a ldc.i4.1 None
0e3b callvirt 0x600150b MyControlCenter.MqttClientCtrl.Publish
0e40 leave.s 3653
0e42 pop None
0e43 leave.s 3653
0e45 ret None

 0x6000ce0 MyControlCenter.NvramVariable.isSupportECALogo recovered
000c ldc.i4.0 None
000d stloc.0 None
000e ldsfld 0x400136b MyControlCenter.NvramVariable.EcCtrl
0013 ldsfld 0x400136c MyControlCenter.NvramVariable.m_ClassName
0018 ldc.i4 1963
001d ldloca.s local(0x0000)
001f callvirt 0x6000621 MyECIO.MyEcCtrl.Read
0024 ldsfld 0x400280e 	..	
0029 ldc.i4 129
002e ldelem.ref None
002f ldloc.0 None
0030 callvirt 0x60020b5 	.

.Invoke
0035 ldc.i4.1 None
0036 conv.i8 None
0037 and None
0038 conv.u1 None
0039 ldc.i4.1 None
003a bne.un.s 77
003c ldstr '0\ufff7\ufffa\x08\x0b\x13\x05\ufff5\x0b\x02\x02\x03\ufffb=<\x16\x19\ufff6￤\x0c6\ufffe\uffe7\x06\uffdf￡￠\ufff6￠\uffd9\t\x0e\x0b\r\uffef￦￭%\uffd8#ￎￓￍￜ'
0041 call 0x6001c6f 	.	.	
0046 call 0x60006e7 Utility.LogCtrl.Write
004b ldc.i4.1 None
004c ret None
004d ldstr '1\ufff8\ufffb\x07\n\x14\x04\ufff6\n\x01\x03\x04\ufffa>=\x15\x18\ufff5￥\x0b7�￦\x05\uffde￢￡\ufff5￡ￚ\x08\r\n\x0e￮￥￬&\uffd9$￡￡￥\uffd1ￚ'
0052 call 0x6001c6f 	.	.	
0057 call 0x60006e7 Utility.LogCtrl.Write
005c ldc.i4.0 None
005d ret None

 0x60011f9 MyControlCenter.MySettingManager.GetBatteryLogoStatus recovered
000c ldc.i4.0 None
000d stloc.0 None
000e ldc.i4.s 64
0010 stloc.2 None
0011 ldarg.0 None
0012 ldfld 0x400192b MyControlCenter.MySettingManager.EcCtrl
0017 ldarg.0 None
0018 ldfld 0x4001920 MyControlCenter.MySettingManager.m_className
001d ldc.i4 1989
0022 ldloca.s local(0x0000)
0024 callvirt 0x6000621 MyECIO.MyEcCtrl.Read
0029 ldloc.0 None
002a ldloc.2 None
002b and None
002c conv.u1 None
002d ldc.i4.s 64
002f bne.un.s 53
0031 ldc.i4.1 None
0032 stloc.1 None
0033 br.s 55
0035 ldc.i4.0 None
0036 stloc.1 None
0037 ldarg.0 None
0038 ldfld 0x4001920 MyControlCenter.MySettingManager.m_className
003d ldstr '\n�\x1e.\uffff\x1f+*\x18*"￮\n\x11\x08\x03!\x13\x1f\x1d\x1e\ufff3O'
0042 call 0x6001c6f 	.	.	
0047 ldsfld 0x400280e 	..	
004c ldc.i4 55
0051 ldelem.ref None
0052 ldloca.s local(0x0001)
0054 callvirt 0x6002021 	.	.Invoke
0059 call 0xa000011 Concat
005e call 0x60006e7 Utility.LogCtrl.Write
0063 ldloc.1 None
0064 ret None

 0x60011fa MyControlCenter.MySettingManager.SetBatteryLogo recovered
000c ldarg.1 None
000d ldc.i4.1 None
000e ceq None
0010 stloc.0 None
0011 ldc.i4 -2147483646
0016 ldarg.0 None
0017 ldfld 0x4001926 MyControlCenter.MySettingManager.m_sRegistryPath
001c ldstr '\x01\x1d10\x1e, \ufff4\x14\x1b\x12\x05(\t%\x11\x07'
0021 call 0x6001c6f 	.	.	
0026 ldloc.0 None
0027 box 0x1000012 System.Int32
002c ldc.i4.4 None
002d call 0x60006ff Utility.RegistryCtrl.RegistrySoftwareKeyWrite
0032 ldarg.1 None
0033 brfalse.s 65
0035 ldstr '\x05\x01\r\x0c\ufffa\x10\x04\ufff0\ufff0\ufff7￮�\x03￫\ufff2\ufff1\uffe7￭\ufff6￥￥'
003a call 0x6001c6f 	.	.	
003f br.s 75
0041 ldstr '\x02\x04\x0e\r�\r\x07\ufff1\uffef\ufff6￭￼\x04￪\ufff1\ufff0￨\ufff0\ufff5￤￪￩'
0046 call 0x6001c6f 	.	.	
004b stsfld 0x400190d MyControlCenter.MySettingParams.sBatteryLogo_Status
0050 ldc.i4.0 None
0051 stloc.1 None
0052 ldarg.0 None
0053 ldfld 0x400192b MyControlCenter.MySettingManager.EcCtrl
0058 ldarg.0 None
0059 ldfld 0x4001920 MyControlCenter.MySettingManager.m_className
005e ldc.i4 1989
0063 ldloca.s local(0x0001)
0065 callvirt 0x6000621 MyECIO.MyEcCtrl.Read
006a ldsfld 0x400280e 	..	
006f ldc.i4 129
0074 ldelem.ref None
0075 ldloc.1 None
0076 callvirt 0x60020b5 	.

.Invoke
007b conv.u1 None
007c stloc.2 None
007d ldarg.1 None
007e ldc.i4.1 None
007f bne.un.s 137
0081 ldloc.2 None
0082 ldc.i4.s 64
0084 or None
0085 conv.u1 None
0086 stloc.2 None
0087 br.s 146
0089 ldloc.2 None
008a ldc.i4 191
008f and None
0090 conv.u1 None
0091 stloc.2 None
0092 ldarg.0 None
0093 ldfld 0x400192b MyControlCenter.MySettingManager.EcCtrl
0098 ldarg.0 None
0099 ldfld 0x4001920 MyControlCenter.MySettingManager.m_className
009e ldc.i4 1989
00a3 ldloc.2 None
00a4 callvirt 0x6000622 MyECIO.MyEcCtrl.Write
00a9 ret None
