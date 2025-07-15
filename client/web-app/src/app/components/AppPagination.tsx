'use client'

import { Pagination } from 'flowbite-react'
import React from 'react'

type Props = {
    currentPage: number,
    totalPage: number,
    onPageChanged: (pageIndex: number) => void; 
}

export default function AppPagination({currentPage, totalPage, onPageChanged}: Props) {
    return (
        <Pagination 
            currentPage={currentPage}
            onPageChange={(e) => onPageChanged(e)}
            totalPages={totalPage}
            layout='pagination'
            showIcons={true}
            className='text-blue-500 mb-5'
        />
    )
}
