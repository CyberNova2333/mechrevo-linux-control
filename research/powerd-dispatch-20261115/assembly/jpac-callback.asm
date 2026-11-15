
/usr/bin/nvidia-powerd:     file format elf64-x86-64


Disassembly of section .text:

0000000000512c90 <.text+0x10ce20>:
  512c90:	41 57                	push   %r15
  512c92:	41 56                	push   %r14
  512c94:	41 55                	push   %r13
  512c96:	41 54                	push   %r12
  512c98:	4c 8d a7 b0 00 00 00 	lea    0xb0(%rdi),%r12
  512c9f:	55                   	push   %rbp
  512ca0:	48 89 fd             	mov    %rdi,%rbp
  512ca3:	53                   	push   %rbx
  512ca4:	48 83 ec 58          	sub    $0x58,%rsp
  512ca8:	48 83 3d 38 53 14 00 	cmpq   $0x0,0x145338(%rip)        # 657fe8 <strtol@plt+0x252180>
  512caf:	00 
  512cb0:	0f b6 9c 24 90 00 00 	movzbl 0x90(%rsp),%ebx
  512cb7:	00 
  512cb8:	74 10                	je     512cca <strtol@plt+0x10ce62>
  512cba:	4c 89 e7             	mov    %r12,%rdi
  512cbd:	e8 9e 2d ef ff       	call   405a60 <pthread_mutex_lock@plt>
  512cc2:	85 c0                	test   %eax,%eax
  512cc4:	0f 85 25 01 00 00    	jne    512def <strtol@plt+0x10cf87>
  512cca:	48 c7 c0 90 d0 65 00 	mov    $0x65d090,%rax
  512cd1:	48 83 38 03          	cmpq   $0x3,(%rax)
  512cd5:	0f 86 ce 00 00 00    	jbe    512da9 <strtol@plt+0x10cf41>
  512cdb:	4c 8d 74 24 30       	lea    0x30(%rsp),%r14
  512ce0:	4c 8d 6c 24 10       	lea    0x10(%rsp),%r13
  512ce5:	0f b6 c3             	movzbl %bl,%eax
  512ce8:	31 d2                	xor    %edx,%edx
  512cea:	4c 8d 7c 24 40       	lea    0x40(%rsp),%r15
  512cef:	4c 89 ee             	mov    %r13,%rsi
  512cf2:	4c 89 f7             	mov    %r14,%rdi
  512cf5:	89 44 24 0c          	mov    %eax,0xc(%rsp)
  512cf9:	4c 89 7c 24 30       	mov    %r15,0x30(%rsp)
  512cfe:	48 c7 44 24 10 22 00 	movq   $0x22,0x10(%rsp)
  512d05:	00 00 
  512d07:	e8 74 63 08 00       	call   599080 <strtol@plt+0x193218>
  512d0c:	48 8b 54 24 10       	mov    0x10(%rsp),%rdx
  512d11:	48 89 44 24 30       	mov    %rax,0x30(%rsp)
  512d16:	4c 89 f6             	mov    %r14,%rsi
  512d19:	4c 89 ef             	mov    %r13,%rdi
  512d1c:	66 0f 6f 05 6c a1 0b 	movdqa 0xba16c(%rip),%xmm0        # 5cce90 <strtol@plt+0x1c7028>
  512d23:	00 
  512d24:	48 8d 4c 24 0c       	lea    0xc(%rsp),%rcx
  512d29:	48 89 54 24 40       	mov    %rdx,0x40(%rsp)
  512d2e:	ba 64 2e 00 00       	mov    $0x2e64,%edx
  512d33:	0f 11 00             	movups %xmm0,(%rax)
  512d36:	66 0f 6f 05 62 a1 0b 	movdqa 0xba162(%rip),%xmm0        # 5ccea0 <strtol@plt+0x1c7038>
  512d3d:	00 
  512d3e:	66 89 50 20          	mov    %dx,0x20(%rax)
  512d42:	48 8b 54 24 30       	mov    0x30(%rsp),%rdx
  512d47:	0f 11 40 10          	movups %xmm0,0x10(%rax)
  512d4b:	48 8b 44 24 10       	mov    0x10(%rsp),%rax
  512d50:	48 89 44 24 38       	mov    %rax,0x38(%rsp)
  512d55:	c6 04 02 00          	movb   $0x0,(%rdx,%rax,1)
  512d59:	48 8d 15 38 06 0b 00 	lea    0xb0638(%rip),%rdx        # 5c3398 <strtol@plt+0x1bd530>
  512d60:	e8 bb 5a ff ff       	call   508820 <strtol@plt+0x1029b8>
  512d65:	48 8b 7c 24 30       	mov    0x30(%rsp),%rdi
  512d6a:	4c 39 ff             	cmp    %r15,%rdi
  512d6d:	74 0e                	je     512d7d <strtol@plt+0x10cf15>
  512d6f:	48 8b 44 24 40       	mov    0x40(%rsp),%rax
  512d74:	48 8d 70 01          	lea    0x1(%rax),%rsi
  512d78:	e8 f3 a6 04 00       	call   55d470 <strtol@plt+0x157608>
  512d7d:	48 8b 74 24 10       	mov    0x10(%rsp),%rsi
  512d82:	bf 04 00 00 00       	mov    $0x4,%edi
  512d87:	e8 24 32 fd ff       	call   4e5fb0 <strtol@plt+0xe0148>
  512d8c:	48 8b 7c 24 10       	mov    0x10(%rsp),%rdi
  512d91:	48 8d 44 24 20       	lea    0x20(%rsp),%rax
  512d96:	48 39 c7             	cmp    %rax,%rdi
  512d99:	74 0e                	je     512da9 <strtol@plt+0x10cf41>
  512d9b:	48 8b 44 24 20       	mov    0x20(%rsp),%rax
  512da0:	48 8d 70 01          	lea    0x1(%rax),%rsi
  512da4:	e8 c7 a6 04 00       	call   55d470 <strtol@plt+0x157608>
  512da9:	88 9d e6 00 00 00    	mov    %bl,0xe6(%rbp)
  512daf:	48 89 ef             	mov    %rbp,%rdi
  512db2:	e8 09 fe ff ff       	call   512bc0 <strtol@plt+0x10cd58>
  512db7:	48 83 3d 29 52 14 00 	cmpq   $0x0,0x145229(%rip)        # 657fe8 <strtol@plt+0x252180>
  512dbe:	00 
  512dbf:	74 1f                	je     512de0 <strtol@plt+0x10cf78>
  512dc1:	48 83 c4 58          	add    $0x58,%rsp
  512dc5:	4c 89 e7             	mov    %r12,%rdi
  512dc8:	5b                   	pop    %rbx
  512dc9:	5d                   	pop    %rbp
  512dca:	41 5c                	pop    %r12
  512dcc:	41 5d                	pop    %r13
  512dce:	41 5e                	pop    %r14
  512dd0:	41 5f                	pop    %r15
  512dd2:	e9 f9 26 ef ff       	jmp    4054d0 <pthread_mutex_unlock@plt>
  512dd7:	66 0f 1f 84 00 00 00 	nopw   0x0(%rax,%rax,1)
  512dde:	00 00 
  512de0:	48 83 c4 58          	add    $0x58,%rsp
  512de4:	5b                   	pop    %rbx
  512de5:	5d                   	pop    %rbp
  512de6:	41 5c                	pop    %r12
  512de8:	41 5d                	pop    %r13
  512dea:	41 5e                	pop    %r14
  512dec:	41 5f                	pop    %r15
  512dee:	c3                   	ret
  512def:	89 c7                	mov    %eax,%edi
  512df1:	e8 dd d6 f0 ff       	call   4204d3 <strtol@plt+0x1a66b>
  512df6:	e9 6a 54 f0 ff       	jmp    418265 <strtol@plt+0x123fd>
  512dfb:	48 89 c7             	mov    %rax,%rdi
  512dfe:	e9 8a 54 f0 ff       	jmp    41828d <strtol@plt+0x12425>
  512e03:	49 89 c6             	mov    %rax,%r14
  512e06:	e9 91 54 f0 ff       	jmp    41829c <strtol@plt+0x12434>
  512e0b:	49 89 c5             	mov    %rax,%r13
  512e0e:	e9 6f 54 f0 ff       	jmp    418282 <strtol@plt+0x1241a>
  512e13:	66 2e 0f 1f 84 00 00 	cs nopw 0x0(%rax,%rax,1)
  512e1a:	00 00 00 
  512e1d:	0f 1f 00             	nopl   (%rax)
  512e20:	31 c0                	xor    %eax,%eax
  512e22:	c3                   	ret
  512e23:	66 2e 0f 1f 84 00 00 	cs nopw 0x0(%rax,%rax,1)
  512e2a:	00 00 00 
  512e2d:	0f 1f 00             	nopl   (%rax)
  512e30:	48 8b 3f             	mov    (%rdi),%rdi
  512e33:	48 85 ff             	test   %rdi,%rdi
  512e36:	74 10                	je     512e48 <strtol@plt+0x10cfe0>
  512e38:	be 10 00 00 00       	mov    $0x10,%esi
  512e3d:	e9 2e a6 04 00       	jmp    55d470 <strtol@plt+0x157608>
  512e42:	66 0f 1f 44 00 00    	nopw   0x0(%rax,%rax,1)
  512e48:	c3                   	ret
  512e49:	0f 1f 80 00 00 00 00 	nopl   0x0(%rax)
  512e50:	41 55                	push   %r13
  512e52:	41 54                	push   %r12
  512e54:	4c 8d 67 10          	lea    0x10(%rdi),%r12
  512e58:	55                   	push   %rbp
  512e59:	53                   	push   %rbx
  512e5a:	48 83 ec 18          	sub    $0x18,%rsp
  512e5e:	4c 89 27             	mov    %r12,(%rdi)
  512e61:	48 85 f6             	test   %rsi,%rsi
  512e64:	0f 84 8c 00 00 00    	je     512ef6 <strtol@plt+0x10d08e>
  512e6a:	48 89 fb             	mov    %rdi,%rbx
  512e6d:	48 89 f7             	mov    %rsi,%rdi
  512e70:	48 89 f5             	mov    %rsi,%rbp
  512e73:	e8 48 24 ef ff       	call   4052c0 <strlen@plt>
  512e78:	48 89 44 24 08       	mov    %rax,0x8(%rsp)
  512e7d:	49 89 c5             	mov    %rax,%r13
