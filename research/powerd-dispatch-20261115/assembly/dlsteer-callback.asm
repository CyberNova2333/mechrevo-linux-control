
/usr/bin/nvidia-powerd:     file format elf64-x86-64


Disassembly of section .text:

00000000004b9d50 <.text+0xb3ee0>:
  4b9d50:	41 57                	push   %r15
  4b9d52:	41 56                	push   %r14
  4b9d54:	41 55                	push   %r13
  4b9d56:	4c 8d af b0 00 00 00 	lea    0xb0(%rdi),%r13
  4b9d5d:	41 54                	push   %r12
  4b9d5f:	55                   	push   %rbp
  4b9d60:	48 89 fd             	mov    %rdi,%rbp
  4b9d63:	53                   	push   %rbx
  4b9d64:	48 83 ec 48          	sub    $0x48,%rsp
  4b9d68:	48 8b 1d 79 e2 19 00 	mov    0x19e279(%rip),%rbx        # 657fe8 <strtol@plt+0x252180>
  4b9d6f:	44 0f b6 a4 24 80 00 	movzbl 0x80(%rsp),%r12d
  4b9d76:	00 00 
  4b9d78:	48 85 db             	test   %rbx,%rbx
  4b9d7b:	74 10                	je     4b9d8d <strtol@plt+0xb3f25>
  4b9d7d:	4c 89 ef             	mov    %r13,%rdi
  4b9d80:	e8 db bc f4 ff       	call   405a60 <pthread_mutex_lock@plt>
  4b9d85:	85 c0                	test   %eax,%eax
  4b9d87:	0f 85 fc 01 00 00    	jne    4b9f89 <strtol@plt+0xb4121>
  4b9d8d:	48 c7 c0 90 d0 65 00 	mov    $0x65d090,%rax
  4b9d94:	44 88 a5 e2 00 00 00 	mov    %r12b,0xe2(%rbp)
  4b9d9b:	48 8b 00             	mov    (%rax),%rax
  4b9d9e:	45 84 e4             	test   %r12b,%r12b
  4b9da1:	0f 84 f9 00 00 00    	je     4b9ea0 <strtol@plt+0xb4038>
  4b9da7:	48 83 f8 03          	cmp    $0x3,%rax
  4b9dab:	0f 86 c1 00 00 00    	jbe    4b9e72 <strtol@plt+0xb400a>
  4b9db1:	4c 8d 74 24 20       	lea    0x20(%rsp),%r14
  4b9db6:	49 89 e4             	mov    %rsp,%r12
  4b9db9:	4c 8d 7c 24 30       	lea    0x30(%rsp),%r15
  4b9dbe:	31 d2                	xor    %edx,%edx
  4b9dc0:	4c 89 e6             	mov    %r12,%rsi
  4b9dc3:	4c 89 f7             	mov    %r14,%rdi
  4b9dc6:	4c 89 7c 24 20       	mov    %r15,0x20(%rsp)
  4b9dcb:	48 c7 04 24 1b 00 00 	movq   $0x1b,(%rsp)
  4b9dd2:	00 
  4b9dd3:	e8 a8 f2 0d 00       	call   599080 <strtol@plt+0x193218>
  4b9dd8:	48 8b 14 24          	mov    (%rsp),%rdx
  4b9ddc:	48 89 44 24 20       	mov    %rax,0x20(%rsp)
  4b9de1:	4c 89 f6             	mov    %r14,%rsi
  4b9de4:	4c 89 e7             	mov    %r12,%rdi
  4b9de7:	66 0f 6f 05 e1 93 10 	movdqa 0x1093e1(%rip),%xmm0        # 5c31d0 <strtol@plt+0x1bd368>
  4b9dee:	00 
  4b9def:	48 b9 20 6e 6f 77 20 	movabs $0x74636120776f6e20,%rcx
  4b9df6:	61 63 74 
  4b9df9:	48 89 54 24 30       	mov    %rdx,0x30(%rsp)
  4b9dfe:	ba 69 76 00 00       	mov    $0x7669,%edx
  4b9e03:	48 89 48 10          	mov    %rcx,0x10(%rax)
  4b9e07:	66 89 50 18          	mov    %dx,0x18(%rax)
  4b9e0b:	c6 40 1a 65          	movb   $0x65,0x1a(%rax)
  4b9e0f:	0f 11 00             	movups %xmm0,(%rax)
  4b9e12:	48 8b 04 24          	mov    (%rsp),%rax
  4b9e16:	48 8b 54 24 20       	mov    0x20(%rsp),%rdx
  4b9e1b:	48 89 44 24 28       	mov    %rax,0x28(%rsp)
  4b9e20:	c6 04 02 00          	movb   $0x0,(%rdx,%rax,1)
  4b9e24:	48 8d 15 6d 95 10 00 	lea    0x10956d(%rip),%rdx        # 5c3398 <strtol@plt+0x1bd530>
  4b9e2b:	e8 60 d8 f8 ff       	call   447690 <strtol@plt+0x41828>
  4b9e30:	48 8b 7c 24 20       	mov    0x20(%rsp),%rdi
  4b9e35:	4c 39 ff             	cmp    %r15,%rdi
  4b9e38:	74 0e                	je     4b9e48 <strtol@plt+0xb3fe0>
  4b9e3a:	48 8b 44 24 30       	mov    0x30(%rsp),%rax
  4b9e3f:	48 8d 70 01          	lea    0x1(%rax),%rsi
  4b9e43:	e8 28 36 0a 00       	call   55d470 <strtol@plt+0x157608>
  4b9e48:	48 8b 34 24          	mov    (%rsp),%rsi
  4b9e4c:	bf 04 00 00 00       	mov    $0x4,%edi
  4b9e51:	e8 5a c1 02 00       	call   4e5fb0 <strtol@plt+0xe0148>
  4b9e56:	48 8b 3c 24          	mov    (%rsp),%rdi
  4b9e5a:	48 8d 44 24 10       	lea    0x10(%rsp),%rax
  4b9e5f:	48 39 c7             	cmp    %rax,%rdi
  4b9e62:	74 0e                	je     4b9e72 <strtol@plt+0xb400a>
  4b9e64:	48 8b 44 24 10       	mov    0x10(%rsp),%rax
  4b9e69:	48 8d 70 01          	lea    0x1(%rax),%rsi
  4b9e6d:	e8 fe 35 0a 00       	call   55d470 <strtol@plt+0x157608>
  4b9e72:	48 89 ef             	mov    %rbp,%rdi
  4b9e75:	e8 26 f9 ff ff       	call   4b97a0 <strtol@plt+0xb3938>
  4b9e7a:	48 85 db             	test   %rbx,%rbx
  4b9e7d:	0f 84 f7 00 00 00    	je     4b9f7a <strtol@plt+0xb4112>
  4b9e83:	48 83 c4 48          	add    $0x48,%rsp
  4b9e87:	4c 89 ef             	mov    %r13,%rdi
  4b9e8a:	5b                   	pop    %rbx
  4b9e8b:	5d                   	pop    %rbp
  4b9e8c:	41 5c                	pop    %r12
  4b9e8e:	41 5d                	pop    %r13
  4b9e90:	41 5e                	pop    %r14
  4b9e92:	41 5f                	pop    %r15
  4b9e94:	e9 37 b6 f4 ff       	jmp    4054d0 <pthread_mutex_unlock@plt>
  4b9e99:	0f 1f 80 00 00 00 00 	nopl   0x0(%rax)
  4b9ea0:	48 83 f8 03          	cmp    $0x3,%rax
  4b9ea4:	0f 86 bf 00 00 00    	jbe    4b9f69 <strtol@plt+0xb4101>
  4b9eaa:	4c 8d 74 24 20       	lea    0x20(%rsp),%r14
  4b9eaf:	49 89 e4             	mov    %rsp,%r12
  4b9eb2:	4c 8d 7c 24 30       	lea    0x30(%rsp),%r15
  4b9eb7:	31 d2                	xor    %edx,%edx
  4b9eb9:	4c 89 e6             	mov    %r12,%rsi
  4b9ebc:	4c 89 f7             	mov    %r14,%rdi
  4b9ebf:	4c 89 7c 24 20       	mov    %r15,0x20(%rsp)
  4b9ec4:	48 c7 04 24 1d 00 00 	movq   $0x1d,(%rsp)
  4b9ecb:	00 
  4b9ecc:	e8 af f1 0d 00       	call   599080 <strtol@plt+0x193218>
  4b9ed1:	48 8b 14 24          	mov    (%rsp),%rdx
  4b9ed5:	48 89 44 24 20       	mov    %rax,0x20(%rsp)
  4b9eda:	4c 89 f6             	mov    %r14,%rsi
  4b9edd:	4c 89 e7             	mov    %r12,%rdi
  4b9ee0:	66 0f 6f 05 e8 92 10 	movdqa 0x1092e8(%rip),%xmm0        # 5c31d0 <strtol@plt+0x1bd368>
  4b9ee7:	00 
  4b9ee8:	48 b9 20 6e 6f 77 20 	movabs $0x616e6920776f6e20,%rcx
  4b9eef:	69 6e 61 
  4b9ef2:	48 89 54 24 30       	mov    %rdx,0x30(%rsp)
  4b9ef7:	48 89 48 10          	mov    %rcx,0x10(%rax)
  4b9efb:	c7 40 18 63 74 69 76 	movl   $0x76697463,0x18(%rax)
  4b9f02:	c6 40 1c 65          	movb   $0x65,0x1c(%rax)
  4b9f06:	0f 11 00             	movups %xmm0,(%rax)
  4b9f09:	48 8b 04 24          	mov    (%rsp),%rax
  4b9f0d:	48 8b 54 24 20       	mov    0x20(%rsp),%rdx
  4b9f12:	48 89 44 24 28       	mov    %rax,0x28(%rsp)
  4b9f17:	c6 04 02 00          	movb   $0x0,(%rdx,%rax,1)
  4b9f1b:	48 8d 15 76 94 10 00 	lea    0x109476(%rip),%rdx        # 5c3398 <strtol@plt+0x1bd530>
  4b9f22:	e8 69 d7 f8 ff       	call   447690 <strtol@plt+0x41828>
  4b9f27:	48 8b 7c 24 20       	mov    0x20(%rsp),%rdi
  4b9f2c:	4c 39 ff             	cmp    %r15,%rdi
  4b9f2f:	74 0e                	je     4b9f3f <strtol@plt+0xb40d7>
  4b9f31:	48 8b 44 24 30       	mov    0x30(%rsp),%rax
  4b9f36:	48 8d 70 01          	lea    0x1(%rax),%rsi
  4b9f3a:	e8 31 35 0a 00       	call   55d470 <strtol@plt+0x157608>
  4b9f3f:	48 8b 34 24          	mov    (%rsp),%rsi
  4b9f43:	bf 04 00 00 00       	mov    $0x4,%edi
  4b9f48:	e8 63 c0 02 00       	call   4e5fb0 <strtol@plt+0xe0148>
  4b9f4d:	48 8b 3c 24          	mov    (%rsp),%rdi
  4b9f51:	48 8d 44 24 10       	lea    0x10(%rsp),%rax
  4b9f56:	48 39 c7             	cmp    %rax,%rdi
  4b9f59:	74 0e                	je     4b9f69 <strtol@plt+0xb4101>
  4b9f5b:	48 8b 44 24 10       	mov    0x10(%rsp),%rax
  4b9f60:	48 8d 70 01          	lea    0x1(%rax),%rsi
  4b9f64:	e8 07 35 0a 00       	call   55d470 <strtol@plt+0x157608>
  4b9f69:	48 89 ef             	mov    %rbp,%rdi
  4b9f6c:	e8 0f f4 ff ff       	call   4b9380 <strtol@plt+0xb3518>
  4b9f71:	48 85 db             	test   %rbx,%rbx
  4b9f74:	0f 85 09 ff ff ff    	jne    4b9e83 <strtol@plt+0xb401b>
  4b9f7a:	48 83 c4 48          	add    $0x48,%rsp
  4b9f7e:	5b                   	pop    %rbx
  4b9f7f:	5d                   	pop    %rbp
  4b9f80:	41 5c                	pop    %r12
  4b9f82:	41 5d                	pop    %r13
  4b9f84:	41 5e                	pop    %r14
  4b9f86:	41 5f                	pop    %r15
  4b9f88:	c3                   	ret
  4b9f89:	89 c7                	mov    %eax,%edi
  4b9f8b:	e8 43 65 f6 ff       	call   4204d3 <strtol@plt+0x1a66b>
  4b9f90:	48 89 c7             	mov    %rax,%rdi
  4b9f93:	e9 f1 84 f5 ff       	jmp    412489 <strtol@plt+0xc621>
  4b9f98:	49 89 c4             	mov    %rax,%r12
  4b9f9b:	e9 de 84 f5 ff       	jmp    41247e <strtol@plt+0xc616>
  4b9fa0:	e9 f3 84 f5 ff       	jmp    412498 <strtol@plt+0xc630>
  4b9fa5:	49 89 c6             	mov    %rax,%r14
  4b9fa8:	e9 03 85 f5 ff       	jmp    4124b0 <strtol@plt+0xc648>
  4b9fad:	49 89 c4             	mov    %rax,%r12
