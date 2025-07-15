'use client'

import { Dropdown, DropdownItem } from 'flowbite-react'
import { User } from 'next-auth';
import Link from 'next/link'
import React from 'react'
import { AiFillCar, AiFillTrophy, AiOutlineLogout } from 'react-icons/ai';
import { HiCog, HiUser } from 'react-icons/hi';
import { signOut } from 'next-auth/react'

type Props = {
    user: User
};

const UserActions = ({ user }: Props) => {
    return (
        <Dropdown inline label={`Welcome ${user.name}`} className='cursor-pointer'>
            <DropdownItem icon={HiUser}>
                My Auctions
            </DropdownItem>
            <DropdownItem icon={AiFillTrophy}>
                Auction won
            </DropdownItem>
            <DropdownItem icon={AiFillCar}>
                Sell my car
            </DropdownItem>
            <DropdownItem icon={HiCog}>
                <Link href='/session'>
                    Session (dev only!)
                </Link>
            </DropdownItem>
            <DropdownItem icon={AiOutlineLogout} onClick={() => signOut({ callbackUrl: '/' })}>
                Sign out
            </DropdownItem>
        </Dropdown>
    )
}

export default UserActions