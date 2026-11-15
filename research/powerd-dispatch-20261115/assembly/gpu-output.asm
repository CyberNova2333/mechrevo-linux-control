
/usr/bin/nvidia-powerd:     file format elf64-x86-64


Disassembly of section .text:

00000000004dc1b0 <.text+0xd6340>:
  4dc1b0:	41 55                	push   %r13
  4dc1b2:	41 89 d0             	mov    %edx,%r8d
  4dc1b5:	49 89 fa             	mov    %rdi,%r10
  4dc1b8:	b9 31 00 00 00       	mov    $0x31,%ecx
  4dc1bd:	41 54                	push   %r12
  4dc1bf:	55                   	push   %rbp
  4dc1c0:	53                   	push   %rbx
  4dc1c1:	48 81 ec a8 02 00 00 	sub    $0x2a8,%rsp
  4dc1c8:	f3 0f 6f 8e b4 01 00 	movdqu 0x1b4(%rsi),%xmm1
  4dc1cf:	00 
  4dc1d0:	f3 0f 6f 96 c4 01 00 	movdqu 0x1c4(%rsi),%xmm2
  4dc1d7:	00 
  4dc1d8:	f3 0f 6f 9e d4 01 00 	movdqu 0x1d4(%rsi),%xmm3
  4dc1df:	00 
  4dc1e0:	48 8b 86 64 02 00 00 	mov    0x264(%rsi),%rax
  4dc1e7:	89 54 24 0c          	mov    %edx,0xc(%rsp)
  4dc1eb:	4c 8d 8c 24 10 01 00 	lea    0x110(%rsp),%r9
  4dc1f2:	00 
  4dc1f3:	f3 0f 6f a6 e4 01 00 	movdqu 0x1e4(%rsi),%xmm4
  4dc1fa:	00 
  4dc1fb:	f3 0f 6f ae f4 01 00 	movdqu 0x1f4(%rsi),%xmm5
  4dc202:	00 
  4dc203:	0f 11 4c 24 50       	movups %xmm1,0x50(%rsp)
  4dc208:	4c 89 cf             	mov    %r9,%rdi
  4dc20b:	f3 0f 6f b6 04 02 00 	movdqu 0x204(%rsi),%xmm6
  4dc212:	00 
  4dc213:	f3 0f 6f be 14 02 00 	movdqu 0x214(%rsi),%xmm7
  4dc21a:	00 
  4dc21b:	0f 11 54 24 60       	movups %xmm2,0x60(%rsp)
  4dc220:	f3 0f 6f 86 24 02 00 	movdqu 0x224(%rsi),%xmm0
  4dc227:	00 
  4dc228:	f3 0f 6f 8e 34 02 00 	movdqu 0x234(%rsi),%xmm1
  4dc22f:	00 
  4dc230:	0f 11 5c 24 70       	movups %xmm3,0x70(%rsp)
  4dc235:	f3 0f 6f 96 44 02 00 	movdqu 0x244(%rsi),%xmm2
  4dc23c:	00 
  4dc23d:	f3 0f 6f 9e 54 02 00 	movdqu 0x254(%rsi),%xmm3
  4dc244:	00 
  4dc245:	48 89 84 24 00 01 00 	mov    %rax,0x100(%rsp)
  4dc24c:	00 
  4dc24d:	31 c0                	xor    %eax,%eax
  4dc24f:	8b 96 14 02 00 00    	mov    0x214(%rsi),%edx
  4dc255:	0f 11 a4 24 80 00 00 	movups %xmm4,0x80(%rsp)
  4dc25c:	00 
  4dc25d:	f3 48 ab             	rep stos %rax,%es:(%rdi)
  4dc260:	0f 11 ac 24 90 00 00 	movups %xmm5,0x90(%rsp)
  4dc267:	00 
  4dc268:	0f 11 b4 24 a0 00 00 	movups %xmm6,0xa0(%rsp)
  4dc26f:	00 
  4dc270:	0f 11 bc 24 b0 00 00 	movups %xmm7,0xb0(%rsp)
  4dc277:	00 
  4dc278:	0f 11 84 24 c0 00 00 	movups %xmm0,0xc0(%rsp)
  4dc27f:	00 
  4dc280:	0f 11 8c 24 d0 00 00 	movups %xmm1,0xd0(%rsp)
  4dc287:	00 
  4dc288:	0f 11 94 24 e0 00 00 	movups %xmm2,0xe0(%rsp)
  4dc28f:	00 
  4dc290:	0f 11 9c 24 f0 00 00 	movups %xmm3,0xf0(%rsp)
  4dc297:	00 
  4dc298:	44 39 c2             	cmp    %r8d,%edx
  4dc29b:	77 73                	ja     4dc310 <strtol@plt+0xd64a8>
  4dc29d:	44 3b 84 24 ac 00 00 	cmp    0xac(%rsp),%r8d
  4dc2a4:	00 
  4dc2a5:	77 69                	ja     4dc310 <strtol@plt+0xd64a8>
  4dc2a7:	41 8b 8a 80 12 00 00 	mov    0x1280(%r10),%ecx
  4dc2ae:	b8 01 00 00 00       	mov    $0x1,%eax
  4dc2b3:	44 89 84 24 1c 01 00 	mov    %r8d,0x11c(%rsp)
  4dc2ba:	00 
  4dc2bb:	ba 34 e6 80 20       	mov    $0x2080e634,%edx
  4dc2c0:	41 b8 88 01 00 00    	mov    $0x188,%r8d
  4dc2c6:	c6 84 24 20 01 00 00 	movb   $0xf7,0x120(%rsp)
  4dc2cd:	f7 
  4dc2ce:	d3 e0                	shl    %cl,%eax
  4dc2d0:	4c 89 c9             	mov    %r9,%rcx
  4dc2d3:	89 84 24 14 01 00 00 	mov    %eax,0x114(%rsp)
  4dc2da:	48 c7 c0 a0 cf 65 00 	mov    $0x65cfa0,%rax
  4dc2e1:	8b 70 10             	mov    0x10(%rax),%esi
  4dc2e4:	48 c7 c0 08 d0 65 00 	mov    $0x65d008,%rax
  4dc2eb:	8b 38                	mov    (%rax),%edi
  4dc2ed:	e8 8e cd f4 ff       	call   429080 <strtol@plt+0x23218>
  4dc2f2:	41 89 c4             	mov    %eax,%r12d
  4dc2f5:	85 c0                	test   %eax,%eax
  4dc2f7:	0f 85 1b 01 00 00    	jne    4dc418 <strtol@plt+0xd65b0>
  4dc2fd:	48 81 c4 a8 02 00 00 	add    $0x2a8,%rsp
  4dc304:	44 89 e0             	mov    %r12d,%eax
  4dc307:	5b                   	pop    %rbx
  4dc308:	5d                   	pop    %rbp
  4dc309:	41 5c                	pop    %r12
  4dc30b:	41 5d                	pop    %r13
  4dc30d:	c3                   	ret
  4dc30e:	66 90                	xchg   %ax,%ax
  4dc310:	48 c7 c0 90 d0 65 00 	mov    $0x65d090,%rax
  4dc317:	41 bc ff ff 00 00    	mov    $0xffff,%r12d
  4dc31d:	48 83 38 01          	cmpq   $0x1,(%rax)
  4dc321:	76 da                	jbe    4dc2fd <strtol@plt+0xd6495>
  4dc323:	4c 8d 6c 24 30       	lea    0x30(%rsp),%r13
  4dc328:	48 8d 6c 24 10       	lea    0x10(%rsp),%rbp
  4dc32d:	31 d2                	xor    %edx,%edx
  4dc32f:	48 c7 44 24 10 2c 00 	movq   $0x2c,0x10(%rsp)
  4dc336:	00 00 
  4dc338:	48 8d 5c 24 40       	lea    0x40(%rsp),%rbx
  4dc33d:	48 89 ee             	mov    %rbp,%rsi
  4dc340:	4c 89 ef             	mov    %r13,%rdi
  4dc343:	48 89 5c 24 30       	mov    %rbx,0x30(%rsp)
  4dc348:	e8 33 cd 0b 00       	call   599080 <strtol@plt+0x193218>
  4dc34d:	48 8b 54 24 10       	mov    0x10(%rsp),%rdx
  4dc352:	48 89 44 24 30       	mov    %rax,0x30(%rsp)
  4dc357:	4c 89 ee             	mov    %r13,%rsi
  4dc35a:	48 89 ef             	mov    %rbp,%rdi
  4dc35d:	66 0f 6f 05 2b b7 0e 	movdqa 0xeb72b(%rip),%xmm0        # 5c7a90 <strtol@plt+0x1c1c28>
  4dc364:	00 
  4dc365:	4c 8d 8c 24 ac 00 00 	lea    0xac(%rsp),%r9
  4dc36c:	00 
  4dc36d:	48 b9 20 25 64 2c 20 	movabs $0x78616d202c642520,%rcx
  4dc374:	6d 61 78 
  4dc377:	4c 8d 84 24 b0 00 00 	lea    0xb0(%rsp),%r8
  4dc37e:	00 
  4dc37f:	48 89 54 24 40       	mov    %rdx,0x40(%rsp)
  4dc384:	0f 11 00             	movups %xmm0,(%rax)
  4dc387:	66 0f 6f 05 11 b7 0e 	movdqa 0xeb711(%rip),%xmm0        # 5c7aa0 <strtol@plt+0x1c1c38>
  4dc38e:	00 
  4dc38f:	48 89 48 20          	mov    %rcx,0x20(%rax)
  4dc393:	48 8d 4c 24 0c       	lea    0xc(%rsp),%rcx
  4dc398:	c7 40 28 3a 20 25 64 	movl   $0x6425203a,0x28(%rax)
  4dc39f:	0f 11 40 10          	movups %xmm0,0x10(%rax)
  4dc3a3:	48 8b 44 24 10       	mov    0x10(%rsp),%rax
  4dc3a8:	48 8b 54 24 30       	mov    0x30(%rsp),%rdx
  4dc3ad:	48 89 44 24 38       	mov    %rax,0x38(%rsp)
  4dc3b2:	c6 04 02 00          	movb   $0x0,(%rdx,%rax,1)
  4dc3b6:	48 8d 15 db 6f 0e 00 	lea    0xe6fdb(%rip),%rdx        # 5c3398 <strtol@plt+0x1bd530>
  4dc3bd:	e8 5e fc ff ff       	call   4dc020 <strtol@plt+0xd61b8>
  4dc3c2:	48 8b 7c 24 30       	mov    0x30(%rsp),%rdi
  4dc3c7:	48 39 df             	cmp    %rbx,%rdi
  4dc3ca:	74 0e                	je     4dc3da <strtol@plt+0xd6572>
  4dc3cc:	48 8b 44 24 40       	mov    0x40(%rsp),%rax
  4dc3d1:	48 8d 70 01          	lea    0x1(%rax),%rsi
  4dc3d5:	e8 96 10 08 00       	call   55d470 <strtol@plt+0x157608>
  4dc3da:	48 8b 74 24 10       	mov    0x10(%rsp),%rsi
  4dc3df:	bf 02 00 00 00       	mov    $0x2,%edi
  4dc3e4:	e8 c7 9b 00 00       	call   4e5fb0 <strtol@plt+0xe0148>
  4dc3e9:	48 8b 7c 24 10       	mov    0x10(%rsp),%rdi
  4dc3ee:	48 8d 44 24 20       	lea    0x20(%rsp),%rax
  4dc3f3:	48 39 c7             	cmp    %rax,%rdi
  4dc3f6:	74 0e                	je     4dc406 <strtol@plt+0xd659e>
  4dc3f8:	48 8b 44 24 20       	mov    0x20(%rsp),%rax
  4dc3fd:	48 8d 70 01          	lea    0x1(%rax),%rsi
  4dc401:	e8 6a 10 08 00       	call   55d470 <strtol@plt+0x157608>
  4dc406:	41 bc ff ff 00 00    	mov    $0xffff,%r12d
  4dc40c:	e9 ec fe ff ff       	jmp    4dc2fd <strtol@plt+0xd6495>
  4dc411:	0f 1f 80 00 00 00 00 	nopl   0x0(%rax)
  4dc418:	48 c7 c0 90 d0 65 00 	mov    $0x65d090,%rax
  4dc41f:	48 83 38 01          	cmpq   $0x1,(%rax)
  4dc423:	0f 86 d4 fe ff ff    	jbe    4dc2fd <strtol@plt+0xd6495>
  4dc429:	4c 8d 6c 24 30       	lea    0x30(%rsp),%r13
  4dc42e:	48 8d 6c 24 10       	lea    0x10(%rsp),%rbp
  4dc433:	31 d2                	xor    %edx,%edx
  4dc435:	48 c7 44 24 10 22 00 	movq   $0x22,0x10(%rsp)
  4dc43c:	00 00 
  4dc43e:	48 8d 5c 24 40       	lea    0x40(%rsp),%rbx
  4dc443:	48 89 ee             	mov    %rbp,%rsi
  4dc446:	4c 89 ef             	mov    %r13,%rdi
  4dc449:	48 89 5c 24 30       	mov    %rbx,0x30(%rsp)
  4dc44e:	e8 2d cc 0b 00       	call   599080 <strtol@plt+0x193218>
  4dc453:	48 8b 54 24 10       	mov    0x10(%rsp),%rdx
  4dc458:	48 89 44 24 30       	mov    %rax,0x30(%rsp)
  4dc45d:	4c 89 ee             	mov    %r13,%rsi
  4dc460:	48 89 ef             	mov    %rbp,%rdi
  4dc463:	66 0f 6f 05 45 b6 0e 	movdqa 0xeb645(%rip),%xmm0        # 5c7ab0 <strtol@plt+0x1c1c48>
  4dc46a:	00 
  4dc46b:	48 89 54 24 40       	mov    %rdx,0x40(%rsp)
  4dc470:	ba 69 74 00 00       	mov    $0x7469,%edx
  4dc475:	0f 11 00             	movups %xmm0,(%rax)
  4dc478:	66 0f 6f 05 40 b6 0e 	movdqa 0xeb640(%rip),%xmm0        # 5c7ac0 <strtol@plt+0x1c1c58>
  4dc47f:	00 
  4dc480:	66 89 50 20          	mov    %dx,0x20(%rax)
  4dc484:	0f 11 40 10          	movups %xmm0,0x10(%rax)
  4dc488:	48 8b 44 24 10       	mov    0x10(%rsp),%rax
  4dc48d:	48 8b 54 24 30       	mov    0x30(%rsp),%rdx
  4dc492:	48 89 44 24 38       	mov    %rax,0x38(%rsp)
  4dc497:	c6 04 02 00          	movb   $0x0,(%rdx,%rax,1)
  4dc49b:	48 8d 15 f6 6e 0e 00 	lea    0xe6ef6(%rip),%rdx        # 5c3398 <strtol@plt+0x1bd530>
  4dc4a2:	e8 e9 b1 f6 ff       	call   447690 <strtol@plt+0x41828>
  4dc4a7:	48 8b 7c 24 30       	mov    0x30(%rsp),%rdi
  4dc4ac:	48 39 df             	cmp    %rbx,%rdi
  4dc4af:	74 0e                	je     4dc4bf <strtol@plt+0xd6657>
  4dc4b1:	48 8b 44 24 40       	mov    0x40(%rsp),%rax
  4dc4b6:	48 8d 70 01          	lea    0x1(%rax),%rsi
  4dc4ba:	e8 b1 0f 08 00       	call   55d470 <strtol@plt+0x157608>
  4dc4bf:	48 8b 74 24 10       	mov    0x10(%rsp),%rsi
  4dc4c4:	bf 02 00 00 00       	mov    $0x2,%edi
  4dc4c9:	e8 e2 9a 00 00       	call   4e5fb0 <strtol@plt+0xe0148>
  4dc4ce:	48 8b 7c 24 10       	mov    0x10(%rsp),%rdi
  4dc4d3:	48 8d 44 24 20       	lea    0x20(%rsp),%rax
  4dc4d8:	48 39 c7             	cmp    %rax,%rdi
  4dc4db:	0f 84 1c fe ff ff    	je     4dc2fd <strtol@plt+0xd6495>
  4dc4e1:	48 8b 44 24 20       	mov    0x20(%rsp),%rax
  4dc4e6:	48 8d 70 01          	lea    0x1(%rax),%rsi
  4dc4ea:	e8 81 0f 08 00       	call   55d470 <strtol@plt+0x157608>
  4dc4ef:	e9 09 fe ff ff       	jmp    4dc2fd <strtol@plt+0xd6495>
  4dc4f4:	48 89 c3             	mov    %rax,%rbx
  4dc4f7:	e9 cc 7d f3 ff       	jmp    4142c8 <strtol@plt+0xe460>
  4dc4fc:	48 89 c3             	mov    %rax,%rbx
  4dc4ff:	e9 de 7d f3 ff       	jmp    4142e2 <strtol@plt+0xe47a>
  4dc504:	48 89 c7             	mov    %rax,%rdi
  4dc507:	e9 c7 7d f3 ff       	jmp    4142d3 <strtol@plt+0xe46b>
  4dc50c:	48 89 c7             	mov    %rax,%rdi
  4dc50f:	e9 e6 7d f3 ff       	jmp    4142fa <strtol@plt+0xe492>
  4dc514:	48 89 c3             	mov    %rax,%rbx
  4dc517:	e9 d3 7d f3 ff       	jmp    4142ef <strtol@plt+0xe487>
  4dc51c:	48 89 c3             	mov    %rax,%rbx
  4dc51f:	e9 eb 7d f3 ff       	jmp    41430f <strtol@plt+0xe4a7>
  4dc524:	66 2e 0f 1f 84 00 00 	cs nopw 0x0(%rax,%rax,1)
  4dc52b:	00 00 00 
  4dc52e:	66 90                	xchg   %ax,%ax
