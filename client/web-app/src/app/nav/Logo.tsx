'use client'

import React from 'react'
import { AiOutlineCar } from 'react-icons/ai'
import { useParamsStore } from '../hooks/useParamsStore'
import { usePathname, useRouter } from 'next/navigation'

const Logo = () => {
    const router = useRouter();
    const pathName = usePathname();

    const resetParams = useParamsStore((state) => state.resetParams);

    function handleReset() {
        if (pathName !== '/') router.push('/');
        resetParams();
    }

    return (
        <div onClick={handleReset} className='cursor-pointer flex items-center gap-2 text-3xl font-semibold text-red-500'>
            <AiOutlineCar size={34} />
            <div>Car Auctions</div>
        </div>
    )
}

export default Logo