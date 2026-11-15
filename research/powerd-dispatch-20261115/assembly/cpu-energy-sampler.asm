
/usr/bin/nvidia-powerd:     file format elf64-x86-64


Disassembly of section .text:

00000000004dd210 <.text+0xd73a0>:
  4dd210:	41 55                	push   %r13
  4dd212:	41 54                	push   %r12
  4dd214:	55                   	push   %rbp
  4dd215:	bd 00 00 00 00       	mov    $0x0,%ebp
  4dd21a:	53                   	push   %rbx
  4dd21b:	48 89 fb             	mov    %rdi,%rbx
  4dd21e:	48 83 ec 18          	sub    $0x18,%rsp
  4dd222:	e8 19 aa 08 00       	call   567c40 <strtol@plt+0x161dd8>
  4dd227:	4c 8d 6c 24 08       	lea    0x8(%rsp),%r13
  4dd22c:	8b 4b 24             	mov    0x24(%rbx),%ecx
  4dd22f:	8b 7b 08             	mov    0x8(%rbx),%edi
  4dd232:	ba 08 00 00 00       	mov    $0x8,%edx
  4dd237:	4c 89 ee             	mov    %r13,%rsi
  4dd23a:	49 89 c4             	mov    %rax,%r12
  4dd23d:	48 c7 44 24 08 00 00 	movq   $0x0,0x8(%rsp)
  4dd244:	00 00 
  4dd246:	e8 d5 85 f2 ff       	call   405820 <pread@plt>
  4dd24b:	8b 4b 20             	mov    0x20(%rbx),%ecx
  4dd24e:	8b 7b 08             	mov    0x8(%rbx),%edi
  4dd251:	ba 08 00 00 00       	mov    $0x8,%edx
  4dd256:	4c 89 ee             	mov    %r13,%rsi
  4dd259:	48 83 f8 08          	cmp    $0x8,%rax
  4dd25d:	0f 44 6c 24 08       	cmove  0x8(%rsp),%ebp
  4dd262:	48 c7 44 24 08 00 00 	movq   $0x0,0x8(%rsp)
  4dd269:	00 00 
  4dd26b:	e8 b0 85 f2 ff       	call   405820 <pread@plt>
  4dd270:	49 89 c0             	mov    %rax,%r8
  4dd273:	b8 01 00 00 00       	mov    $0x1,%eax
  4dd278:	49 83 f8 08          	cmp    $0x8,%r8
  4dd27c:	75 0b                	jne    4dd289 <strtol@plt+0xd7421>
  4dd27e:	48 8b 4c 24 08       	mov    0x8(%rsp),%rcx
  4dd283:	48 c1 e9 08          	shr    $0x8,%rcx
  4dd287:	d3 e0                	shl    %cl,%eax
  4dd289:	8b 53 10             	mov    0x10(%rbx),%edx
  4dd28c:	85 d2                	test   %edx,%edx
  4dd28e:	75 20                	jne    4dd2b0 <strtol@plt+0xd7448>
  4dd290:	f2 0f 10 03          	movsd  (%rbx),%xmm0
  4dd294:	89 6b 10             	mov    %ebp,0x10(%rbx)
  4dd297:	4c 89 63 18          	mov    %r12,0x18(%rbx)
  4dd29b:	48 83 c4 18          	add    $0x18,%rsp
  4dd29f:	5b                   	pop    %rbx
  4dd2a0:	5d                   	pop    %rbp
  4dd2a1:	41 5c                	pop    %r12
  4dd2a3:	41 5d                	pop    %r13
  4dd2a5:	c3                   	ret
  4dd2a6:	66 2e 0f 1f 84 00 00 	cs nopw 0x0(%rax,%rax,1)
  4dd2ad:	00 00 00 
  4dd2b0:	89 e9                	mov    %ebp,%ecx
  4dd2b2:	66 0f ef c0          	pxor   %xmm0,%xmm0
  4dd2b6:	66 0f ef c9          	pxor   %xmm1,%xmm1
  4dd2ba:	29 d1                	sub    %edx,%ecx
  4dd2bc:	4c 89 e2             	mov    %r12,%rdx
  4dd2bf:	48 2b 53 18          	sub    0x18(%rbx),%rdx
  4dd2c3:	f2 48 0f 2a c1       	cvtsi2sd %rcx,%xmm0
  4dd2c8:	f2 48 0f 2a ca       	cvtsi2sd %rdx,%xmm1
  4dd2cd:	f2 0f 5e 0d 6b 0e 0e 	divsd  0xe0e6b(%rip),%xmm1        # 5be140 <strtol@plt+0x1b82d8>
  4dd2d4:	00 
  4dd2d5:	f2 0f 5e c1          	divsd  %xmm1,%xmm0
  4dd2d9:	66 0f ef c9          	pxor   %xmm1,%xmm1
  4dd2dd:	f2 48 0f 2a c8       	cvtsi2sd %rax,%xmm1
  4dd2e2:	f2 0f 5e c1          	divsd  %xmm1,%xmm0
  4dd2e6:	f2 0f 5a c0          	cvtsd2ss %xmm0,%xmm0
  4dd2ea:	f3 0f 5a c0          	cvtss2sd %xmm0,%xmm0
  4dd2ee:	f2 0f 11 03          	movsd  %xmm0,(%rbx)
  4dd2f2:	eb a0                	jmp    4dd294 <strtol@plt+0xd742c>
  4dd2f4:	66 2e 0f 1f 84 00 00 	cs nopw 0x0(%rax,%rax,1)
  4dd2fb:	00 00 00 
  4dd2fe:	66 90                	xchg   %ax,%ax
  4dd300:	41 54                	push   %r12
  4dd302:	55                   	push   %rbp
  4dd303:	53                   	push   %rbx
  4dd304:	48 89 fb             	mov    %rdi,%rbx
  4dd307:	48 83 ec 50          	sub    $0x50,%rsp
  4dd30b:	e8 50 fe ff ff       	call   4dd160 <strtol@plt+0xd72f8>
  4dd310:	48 8d 6c 24 01       	lea    0x1(%rsp),%rbp
  4dd315:	31 f6                	xor    %esi,%esi
  4dd317:	48 b8 2f 64 65 76 2f 	movabs $0x7570632f7665642f,%rax
  4dd31e:	63 70 75 
  4dd321:	48 89 44 24 01       	mov    %rax,0x1(%rsp)
  4dd326:	b9 73 72 00 00       	mov    $0x7273,%ecx
  4dd32b:	48 89 ef             	mov    %rbp,%rdi
  4dd32e:	31 c0                	xor    %eax,%eax
  4dd330:	c7 44 24 09 2f 30 2f 	movl   $0x6d2f302f,0x9(%rsp)
  4dd337:	6d 
  4dd338:	66 89 4c 24 0d       	mov    %cx,0xd(%rsp)
  4dd33d:	c6 44 24 0f 00       	movb   $0x0,0xf(%rsp)
  4dd342:	e8 39 81 f2 ff       	call   405480 <open@plt>
  4dd347:	89 43 08             	mov    %eax,0x8(%rbx)
  4dd34a:	85 c0                	test   %eax,%eax
  4dd34c:	0f 88 de 00 00 00    	js     4dd430 <strtol@plt+0xd75c8>
  4dd352:	be 02 00 00 00       	mov    $0x2,%esi
  4dd357:	48 89 ef             	mov    %rbp,%rdi
  4dd35a:	31 c0                	xor    %eax,%eax
  4dd35c:	e8 1f 81 f2 ff       	call   405480 <open@plt>
  4dd361:	89 43 0c             	mov    %eax,0xc(%rbx)
  4dd364:	85 c0                	test   %eax,%eax
  4dd366:	0f 88 c4 02 00 00    	js     4dd630 <strtol@plt+0xd77c8>
  4dd36c:	48 8d 53 28          	lea    0x28(%rbx),%rdx
  4dd370:	b9 0c 00 00 00       	mov    $0xc,%ecx
  4dd375:	48 8d 3d 6a 9e 0d 00 	lea    0xd9e6a(%rip),%rdi        # 5b71e6 <strtol@plt+0x1b137e>
  4dd37c:	48 89 d6             	mov    %rdx,%rsi
  4dd37f:	f3 a6                	repz cmpsb %es:(%rdi),%ds:(%rsi)
  4dd381:	0f 97 c0             	seta   %al
  4dd384:	1c 00                	sbb    $0x0,%al
  4dd386:	84 c0                	test   %al,%al
  4dd388:	75 56                	jne    4dd3e0 <strtol@plt+0xd7578>
  4dd38a:	48 b8 06 06 00 00 11 	movabs $0x61100000606,%rax
  4dd391:	06 00 00 
  4dd394:	b9 11 06 00 00       	mov    $0x611,%ecx
  4dd399:	48 89 43 20          	mov    %rax,0x20(%rbx)
  4dd39d:	8b 7b 08             	mov    0x8(%rbx),%edi
  4dd3a0:	48 8d 74 24 30       	lea    0x30(%rsp),%rsi
  4dd3a5:	ba 08 00 00 00       	mov    $0x8,%edx
  4dd3aa:	48 c7 44 24 30 00 00 	movq   $0x0,0x30(%rsp)
  4dd3b1:	00 00 
  4dd3b3:	e8 68 84 f2 ff       	call   405820 <pread@plt>
  4dd3b8:	48 83 f8 08          	cmp    $0x8,%rax
  4dd3bc:	75 5a                	jne    4dd418 <strtol@plt+0xd75b0>
  4dd3be:	48 83 7c 24 30 00    	cmpq   $0x0,0x30(%rsp)
  4dd3c4:	0f 84 76 01 00 00    	je     4dd540 <strtol@plt+0xd76d8>
  4dd3ca:	66 0f ef c0          	pxor   %xmm0,%xmm0
  4dd3ce:	31 c0                	xor    %eax,%eax
  4dd3d0:	0f 11 43 10          	movups %xmm0,0x10(%rbx)
  4dd3d4:	48 83 c4 50          	add    $0x50,%rsp
  4dd3d8:	5b                   	pop    %rbx
  4dd3d9:	5d                   	pop    %rbp
  4dd3da:	41 5c                	pop    %r12
  4dd3dc:	c3                   	ret
  4dd3dd:	0f 1f 00             	nopl   (%rax)
  4dd3e0:	b9 0c 00 00 00       	mov    $0xc,%ecx
  4dd3e5:	48 8d 3d 07 9e 0d 00 	lea    0xd9e07(%rip),%rdi        # 5b71f3 <strtol@plt+0x1b138b>
  4dd3ec:	48 89 d6             	mov    %rdx,%rsi
  4dd3ef:	f3 a6                	repz cmpsb %es:(%rdi),%ds:(%rsi)
  4dd3f1:	0f 97 c0             	seta   %al
  4dd3f4:	1c 00                	sbb    $0x0,%al
  4dd3f6:	84 c0                	test   %al,%al
  4dd3f8:	0f 85 3a 01 00 00    	jne    4dd538 <strtol@plt+0xd76d0>
  4dd3fe:	48 b8 99 02 01 c0 9b 	movabs $0xc001029bc0010299,%rax
  4dd405:	02 01 c0 
  4dd408:	b9 9b 02 01 c0       	mov    $0xc001029b,%ecx
  4dd40d:	48 89 43 20          	mov    %rax,0x20(%rbx)
  4dd411:	eb 8a                	jmp    4dd39d <strtol@plt+0xd7535>
  4dd413:	0f 1f 44 00 00       	nopl   0x0(%rax,%rax,1)
  4dd418:	48 83 c4 50          	add    $0x50,%rsp
  4dd41c:	b8 ff ff 00 00       	mov    $0xffff,%eax
  4dd421:	5b                   	pop    %rbx
  4dd422:	5d                   	pop    %rbp
  4dd423:	41 5c                	pop    %r12
  4dd425:	c3                   	ret
