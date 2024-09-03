'use client';

import { Button, Dropdown, DropdownItem } from 'flowbite-react';
import { User } from 'next-auth';
import { signOut } from 'next-auth/react';
import Link from 'next/link';
import { usePathname, useRouter } from 'next/navigation';
import React from 'react';
import { AiFillCar, AiFillTrophy, AiOutlineLogout } from 'react-icons/ai';
import { HiCog, HiUser } from 'react-icons/hi';
import { useParamsStore } from '../hooks/useParamsStore';

type Props = {
	user: User;
};

export default function UserActions({ user }: Props) {
	const router = useRouter();
	const pathName = usePathname();
	const setParams = useParamsStore((state) => state.setParams);

	function setWinner() {
		setParams({ winner: user.username, seller: undefined });

		if (pathName !== '/') router.push('/');
	}

	function setSeller() {
		setParams({ seller: user.username, winner: undefined });

		if (pathName !== '/') router.push('/');
	}

	return (
		<Dropdown inline label={`Welcome ${user.name}`}>
			<DropdownItem icon={HiUser} onClick={setSeller}>
				My Auctions
				{/* <Link href='/'>My Auctions</Link> */}
			</DropdownItem>
			<DropdownItem icon={AiFillTrophy} onClick={setWinner}>
				Auctions won
				{/* <Link href='/'>Auctions won</Link> */}
			</DropdownItem>
			<DropdownItem icon={AiFillCar}>
				<Link href='/auctions/create'>Sell my car</Link>
			</DropdownItem>
			<DropdownItem icon={HiCog}>
				<Link href='/session'>Session (dev only!)</Link>
			</DropdownItem>
			<DropdownItem
				icon={AiOutlineLogout}
				onClick={() => signOut({ callbackUrl: '/' })}
			>
				<Link href='/'>Sign out</Link>
			</DropdownItem>
		</Dropdown>
	);
}
