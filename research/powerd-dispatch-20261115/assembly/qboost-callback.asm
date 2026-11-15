
/usr/bin/nvidia-powerd:     file format elf64-x86-64


Disassembly of section .text:

00000000004d46c0 <.text+0xce850>:
  4d46c0:	55                   	push   %rbp
  4d46c1:	48 89 e5             	mov    %rsp,%rbp
  4d46c4:	41 56                	push   %r14
  4d46c6:	41 55                	push   %r13
  4d46c8:	41 54                	push   %r12
  4d46ca:	4c 8d a7 b8 00 00 00 	lea    0xb8(%rdi),%r12
  4d46d1:	53                   	push   %rbx
  4d46d2:	48 89 fb             	mov    %rdi,%rbx
  4d46d5:	48 83 ec 40          	sub    $0x40,%rsp
  4d46d9:	48 83 3d 07 39 18 00 	cmpq   $0x0,0x183907(%rip)        # 657fe8 <strtol@plt+0x252180>
  4d46e0:	00 
  4d46e1:	74 10                	je     4d46f3 <strtol@plt+0xce88b>
  4d46e3:	4c 89 e7             	mov    %r12,%rdi
  4d46e6:	e8 75 13 f3 ff       	call   405a60 <pthread_mutex_lock@plt>
  4d46eb:	85 c0                	test   %eax,%eax
  4d46ed:	0f 85 66 01 00 00    	jne    4d4859 <strtol@plt+0xce9f1>
  4d46f3:	48 8b bb b0 00 00 00 	mov    0xb0(%rbx),%rdi
  4d46fa:	48 85 ff             	test   %rdi,%rdi
  4d46fd:	74 61                	je     4d4760 <strtol@plt+0xce8f8>
  4d46ff:	48 8b 45 40          	mov    0x40(%rbp),%rax
  4d4703:	48 83 ec 40          	sub    $0x40,%rsp
  4d4707:	f3 0f 6f 4d 10       	movdqu 0x10(%rbp),%xmm1
  4d470c:	f3 0f 6f 55 20       	movdqu 0x20(%rbp),%xmm2
  4d4711:	f3 0f 6f 5d 30       	movdqu 0x30(%rbp),%xmm3
  4d4716:	48 89 44 24 30       	mov    %rax,0x30(%rsp)
  4d471b:	8b 45 48             	mov    0x48(%rbp),%eax
  4d471e:	0f 11 0c 24          	movups %xmm1,(%rsp)
  4d4722:	89 44 24 38          	mov    %eax,0x38(%rsp)
  4d4726:	0f 11 54 24 10       	movups %xmm2,0x10(%rsp)
  4d472b:	0f 11 5c 24 20       	movups %xmm3,0x20(%rsp)
  4d4730:	e8 cb 4d 00 00       	call   4d9500 <strtol@plt+0xd3698>
  4d4735:	48 83 c4 40          	add    $0x40,%rsp
  4d4739:	48 83 3d a7 38 18 00 	cmpq   $0x0,0x1838a7(%rip)        # 657fe8 <strtol@plt+0x252180>
  4d4740:	00 
  4d4741:	0f 84 05 01 00 00    	je     4d484c <strtol@plt+0xce9e4>
  4d4747:	48 8d 65 e0          	lea    -0x20(%rbp),%rsp
  4d474b:	4c 89 e7             	mov    %r12,%rdi
  4d474e:	5b                   	pop    %rbx
  4d474f:	41 5c                	pop    %r12
  4d4751:	41 5d                	pop    %r13
  4d4753:	41 5e                	pop    %r14
  4d4755:	5d                   	pop    %rbp
  4d4756:	e9 75 0d f3 ff       	jmp    4054d0 <pthread_mutex_unlock@plt>
  4d475b:	0f 1f 44 00 00       	nopl   0x0(%rax,%rax,1)
  4d4760:	48 c7 c0 90 d0 65 00 	mov    $0x65d090,%rax
  4d4767:	48 83 38 01          	cmpq   $0x1,(%rax)
  4d476b:	76 cc                	jbe    4d4739 <strtol@plt+0xce8d1>
  4d476d:	4c 8d 75 c0          	lea    -0x40(%rbp),%r14
  4d4771:	4c 8d 6d a0          	lea    -0x60(%rbp),%r13
  4d4775:	31 d2                	xor    %edx,%edx
  4d4777:	48 c7 45 a0 43 00 00 	movq   $0x43,-0x60(%rbp)
  4d477e:	00 
  4d477f:	48 8d 5d d0          	lea    -0x30(%rbp),%rbx
  4d4783:	4c 89 ee             	mov    %r13,%rsi
  4d4786:	4c 89 f7             	mov    %r14,%rdi
  4d4789:	48 89 5d c0          	mov    %rbx,-0x40(%rbp)
  4d478d:	e8 ee 48 0c 00       	call   599080 <strtol@plt+0x193218>
  4d4792:	48 8b 55 a0          	mov    -0x60(%rbp),%rdx
  4d4796:	48 89 45 c0          	mov    %rax,-0x40(%rbp)
  4d479a:	4c 89 f6             	mov    %r14,%rsi
  4d479d:	4c 89 ef             	mov    %r13,%rdi
  4d47a0:	66 0f 6f 05 38 30 0f 	movdqa 0xf3038(%rip),%xmm0        # 5c77e0 <strtol@plt+0x1c1978>
  4d47a7:	00 
  4d47a8:	c6 40 42 2e          	movb   $0x2e,0x42(%rax)
  4d47ac:	48 89 55 d0          	mov    %rdx,-0x30(%rbp)
  4d47b0:	ba 73 74 00 00       	mov    $0x7473,%edx
  4d47b5:	0f 11 00             	movups %xmm0,(%rax)
  4d47b8:	66 0f 6f 05 30 30 0f 	movdqa 0xf3030(%rip),%xmm0        # 5c77f0 <strtol@plt+0x1c1988>
  4d47bf:	00 
  4d47c0:	66 89 50 40          	mov    %dx,0x40(%rax)
  4d47c4:	48 8b 55 c0          	mov    -0x40(%rbp),%rdx
  4d47c8:	0f 11 40 10          	movups %xmm0,0x10(%rax)
  4d47cc:	66 0f 6f 05 2c 30 0f 	movdqa 0xf302c(%rip),%xmm0        # 5c7800 <strtol@plt+0x1c1998>
  4d47d3:	00 
  4d47d4:	0f 11 40 20          	movups %xmm0,0x20(%rax)
  4d47d8:	66 0f 6f 05 30 30 0f 	movdqa 0xf3030(%rip),%xmm0        # 5c7810 <strtol@plt+0x1c19a8>
  4d47df:	00 
  4d47e0:	0f 11 40 30          	movups %xmm0,0x30(%rax)
  4d47e4:	48 8b 45 a0          	mov    -0x60(%rbp),%rax
  4d47e8:	48 89 45 c8          	mov    %rax,-0x38(%rbp)
  4d47ec:	c6 04 02 00          	movb   $0x0,(%rdx,%rax,1)
  4d47f0:	48 8d 15 a1 eb 0e 00 	lea    0xeeba1(%rip),%rdx        # 5c3398 <strtol@plt+0x1bd530>
  4d47f7:	e8 94 2e f7 ff       	call   447690 <strtol@plt+0x41828>
  4d47fc:	48 8b 7d c0          	mov    -0x40(%rbp),%rdi
  4d4800:	48 39 df             	cmp    %rbx,%rdi
  4d4803:	74 0d                	je     4d4812 <strtol@plt+0xce9aa>
  4d4805:	48 8b 45 d0          	mov    -0x30(%rbp),%rax
  4d4809:	48 8d 70 01          	lea    0x1(%rax),%rsi
  4d480d:	e8 5e 8c 08 00       	call   55d470 <strtol@plt+0x157608>
  4d4812:	48 8b 75 a0          	mov    -0x60(%rbp),%rsi
  4d4816:	bf 02 00 00 00       	mov    $0x2,%edi
  4d481b:	e8 90 17 01 00       	call   4e5fb0 <strtol@plt+0xe0148>
  4d4820:	48 8b 7d a0          	mov    -0x60(%rbp),%rdi
  4d4824:	48 8d 45 b0          	lea    -0x50(%rbp),%rax
  4d4828:	48 39 c7             	cmp    %rax,%rdi
  4d482b:	0f 84 08 ff ff ff    	je     4d4739 <strtol@plt+0xce8d1>
  4d4831:	48 8b 45 b0          	mov    -0x50(%rbp),%rax
  4d4835:	48 8d 70 01          	lea    0x1(%rax),%rsi
  4d4839:	e8 32 8c 08 00       	call   55d470 <strtol@plt+0x157608>
  4d483e:	48 83 3d a2 37 18 00 	cmpq   $0x0,0x1837a2(%rip)        # 657fe8 <strtol@plt+0x252180>
  4d4845:	00 
  4d4846:	0f 85 fb fe ff ff    	jne    4d4747 <strtol@plt+0xce8df>
  4d484c:	48 8d 65 e0          	lea    -0x20(%rbp),%rsp
  4d4850:	5b                   	pop    %rbx
  4d4851:	41 5c                	pop    %r12
  4d4853:	41 5d                	pop    %r13
  4d4855:	41 5e                	pop    %r14
  4d4857:	5d                   	pop    %rbp
  4d4858:	c3                   	ret
  4d4859:	89 c7                	mov    %eax,%edi
  4d485b:	e8 73 bc f4 ff       	call   4204d3 <strtol@plt+0x1a66b>
  4d4860:	48 89 c3             	mov    %rax,%rbx
  4d4863:	e9 cc f1 f3 ff       	jmp    413a34 <strtol@plt+0xdbcc>
  4d4868:	48 89 c3             	mov    %rax,%rbx
  4d486b:	e9 de f1 f3 ff       	jmp    413a4e <strtol@plt+0xdbe6>
  4d4870:	48 89 c7             	mov    %rax,%rdi
  4d4873:	e9 c7 f1 f3 ff       	jmp    413a3f <strtol@plt+0xdbd7>
  4d4878:	e9 de f1 f3 ff       	jmp    413a5b <strtol@plt+0xdbf3>
  4d487d:	0f 1f 00             	nopl   (%rax)
